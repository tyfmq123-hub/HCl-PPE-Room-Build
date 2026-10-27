"""Repository integrity checks only; this does not compile or execute Unity code."""
import argparse
import hashlib
import json
import re
import subprocess
from datetime import datetime, timezone
from pathlib import Path
from urllib.parse import unquote

ROOT = Path(__file__).resolve().parents[3]


def git(*args):
    return subprocess.check_output(["git", *args], cwd=ROOT, text=True, encoding="utf-8").strip()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path)
    options = parser.parse_args()
    checks = []

    def check(name, failures, **details):
        checks.append({"name": name, "status": "PASS" if not failures else "FAIL",
                       "failures": failures, **details})

    baseline = git("rev-parse", "HEAD")
    branch = git("branch", "--show-current")
    check("requested_branch", [] if branch == "codex/ppe-portfolio-improvement" else [branch])
    tracked = git("ls-files").splitlines()
    changes = git("diff", "--name-only", "--no-ext-diff").splitlines()
    allowed = ("Assets/1.JSR/", "docs/portfolio/", "docs/handoffs/ppe/LATEST.md", "README.md")
    unexpected = [path for path in changes if not path.startswith(allowed)]
    untracked = git("ls-files", "--others", "--exclude-standard").splitlines()
    unexpected += [path for path in untracked if not path.startswith(allowed)]
    check("change_ownership", unexpected, modified=changes, added=untracked)

    invalid_json = []
    json_files = [ROOT / "Packages/manifest.json", ROOT / "Packages/packages-lock.json", ROOT / "EXPORT-MANIFEST.json"]
    for file in json_files:
        try:
            json.loads(file.read_text(encoding="utf-8-sig"))
        except (ValueError, OSError) as error:
            invalid_json.append(f"{file.relative_to(ROOT)}: {error}")
    check("configuration_json", invalid_json, files=len(json_files))

    lfs_pointers = []
    lfs_files = git("lfs", "ls-files", "--name-only").splitlines()
    for path in lfs_files:
        file = ROOT / path
        if not file.exists():
            lfs_pointers.append(path + " (missing)")
            continue
        with file.open("rb") as stream:
            if stream.read(42).startswith(b"version https://git-lfs.github.com/spec/"):
                lfs_pointers.append(path)
    check("lfs_materialized", lfs_pointers, files=len(lfs_files))

    manifest = json.loads((ROOT / "EXPORT-MANIFEST.json").read_text(encoding="utf-8-sig"))
    runtime_files = [entry for entry in manifest["files"] if entry["path"].startswith("Runtime/")]
    runtime_errors = []
    for entry in runtime_files:
        file = ROOT / entry["path"]
        if not file.is_file():
            runtime_errors.append(entry["path"] + " (missing)")
            continue
        with file.open("rb") as stream:
            actual = hashlib.file_digest(stream, "sha256").hexdigest()
        if actual != entry["sha256"]:
            runtime_errors.append(entry["path"])
    check("original_runtime_sha256", runtime_errors, files=len(runtime_files),
          historical_build_date=manifest["existing_build_date"])

    meta_errors = []
    metas = list((ROOT / "Assets/1.JSR").rglob("*.meta"))
    guid_paths = {}
    # Include all source asset GUIDs so a newly added test cannot collide with another Stage.
    for meta in (ROOT / "Assets").rglob("*.meta"):
        match = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(encoding="utf-8-sig"), re.M)
        if match:
            guid_paths.setdefault(match[1], []).append(str(meta.relative_to(ROOT)))
    for file in (ROOT / "Assets/1.JSR").rglob("*.cs"):
        meta = file.with_suffix(file.suffix + ".meta")
        if not meta.exists():
            meta_errors.append(str(meta.relative_to(ROOT)) + " (missing)")
        else:
            match = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(encoding="utf-8-sig"), re.M)
            if not match:
                meta_errors.append(str(meta.relative_to(ROOT)) + " (invalid GUID)")
            elif len(guid_paths[match[1]]) != 1:
                meta_errors.append(guid_paths[match[1]])
    for path in changes:
        if path.endswith(".meta"):
            before = git("show", "HEAD:" + path)
            after = (ROOT / path).read_text(encoding="utf-8-sig")
            old_guid = re.search(r"^guid: (.+)$", before, re.M)
            new_guid = re.search(r"^guid: (.+)$", after, re.M)
            if old_guid and (not new_guid or old_guid[1].strip() != new_guid[1].strip()):
                meta_errors.append(path + " (existing GUID changed)")
    check("ppe_script_meta_and_guids", meta_errors, ppe_meta_files=len(metas))

    fixture_cases = {}
    for name in ("PPEPortfolioSequenceTests", "PPEGlovePairStateTests", "ResponseBagPackingTests"):
        source = (ROOT / "Assets/1.JSR/Editor" / (name + ".cs")).read_text(encoding="utf-8-sig")
        fixture_cases[name] = len(re.findall(r"^\s*\[Test\]\s*$", source, re.M)) + len(
            re.findall(r"^\s*\[TestCase\(", source, re.M))
    check("authored_test_source_inventory", [], fixtures=fixture_cases,
          total_authored_cases=sum(fixture_cases.values()), execution="NOT RUN")

    files = [ROOT / "README.md", *sorted((ROOT / "docs/portfolio").glob("*.md"))]
    broken_links = []
    link_count = 0
    for file in files:
        for target in re.findall(r"\]\(([^)]+)\)", file.read_text(encoding="utf-8-sig")):
            target = target.strip().strip("<>")
            if re.match(r"[a-zA-Z][a-zA-Z0-9+.-]*:", target) or target.startswith("#"):
                continue
            link_count += 1
            path = unquote(target.split("#", 1)[0])
            destination = (file.parent / path).resolve()
            is_report_being_written = options.output and destination == options.output.resolve()
            if path and not destination.exists() and not is_report_being_written:
                broken_links.append(f"{file.relative_to(ROOT)}: {target}")
    check("portfolio_local_links", broken_links, files=len(files), links=link_count)

    report = {"checked_at_utc": datetime.now(timezone.utc).isoformat(),
              "baseline_commit": baseline, "branch": branch,
              "scope": "Static repository integrity only. Unity compilation, behavior tests and device execution NOT VERIFIED.",
              "checks": checks}
    rendered = json.dumps(report, ensure_ascii=False, indent=2)
    if options.output:
        destination = options.output.resolve()
        destination.parent.mkdir(parents=True, exist_ok=True)
        destination.write_text(rendered + "\n", encoding="utf-8")
    print(rendered)
    return int(any(item["status"] == "FAIL" for item in checks))


if __name__ == "__main__":
    raise SystemExit(main())
