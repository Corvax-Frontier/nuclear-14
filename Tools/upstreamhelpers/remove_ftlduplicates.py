import os
import re

def normalize_text(text):
    return re.sub(r'\{\$[a-zA-Z0-9_-]+\}', '{PLACEHOLDER}', text).strip()

def count_braces(s):
    return s.count('{') - s.count('}')

def remove_ftl_duplicates(directory):
    total_removed = 0
    entry_start = re.compile(r'^([a-zA-Z0-9_-]+)\s*=\s*(.*)$')
    attribute_line = re.compile(r'^\s*\.(\w+)\s*=\s*(.*)$')

    seen_ids = set()

    for root, _, files in os.walk(directory):
        for file in files:
            if not file.endswith('.ftl'):
                continue

            path = os.path.join(root, file)
            with open(path, 'r', encoding='utf-8') as f:
                lines = f.readlines()

            new_lines = []
            current_entry_id = None
            buffer = []
            brace_balance = 0
            duplicate_count = 0

            def flush_buffer(force=False):
                nonlocal buffer, current_entry_id, duplicate_count
                if not buffer:
                    return
                if brace_balance != 0 and not force:
                    return
                normalized = normalize_text("".join(buffer))
                if current_entry_id in seen_ids:
                    print(f"[DUPLICATE] {path} → ID '{current_entry_id}' removed")
                    duplicate_count += 1
                else:
                    seen_ids.add(current_entry_id)
                    new_lines.extend(buffer)
                buffer = []
                current_entry_id = None

            for line in lines:
                stripped = line.strip()

                if stripped == '' or stripped.startswith('#'):
                    flush_buffer(force=True)
                    new_lines.append(line)
                    continue

                entry_match = entry_start.match(stripped)
                attr_match = attribute_line.match(stripped)

                if entry_match:
                    flush_buffer(force=True)
                    current_entry_id = entry_match.group(1).strip()
                    buffer = [line]
                    brace_balance = count_braces(line)
                    continue

                elif attr_match and current_entry_id:
                    current_attr_id = f"{current_entry_id}.{attr_match.group(1).strip()}"
                    if current_attr_id in seen_ids:
                        print(f"[DUPLICATE] {path} → ID '{current_attr_id}' removed")
                        duplicate_count += 1
                        continue
                    buffer.append(line)
                    seen_ids.add(current_attr_id)
                    brace_balance = count_braces(line)
                    continue

                if buffer:
                    buffer.append(line)
                    brace_balance += count_braces(line)
                    if brace_balance == 0:
                        flush_buffer()
                else:
                    new_lines.append(line)

            flush_buffer(force=True)

            if duplicate_count > 0:
                with open(path, 'w', encoding='utf-8') as f:
                    f.writelines(new_lines)
                print(f"[CLEANED] {path} — Removed {duplicate_count} duplicate(s)")
                total_removed += duplicate_count

    print(f"\n[SUMMARY] Total duplicate entries removed: {total_removed}")

if __name__ == '__main__':
    import argparse
    parser = argparse.ArgumentParser(description='Remove duplicate entries from .ftl files.')
    parser.add_argument('directory', help='Path to directory containing .ftl files')
    args = parser.parse_args()

    remove_ftl_duplicates(args.directory)
