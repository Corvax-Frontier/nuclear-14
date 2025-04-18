import io
import os
import argparse
from ruamel.yaml import YAML
from ruamel.yaml.comments import CommentedMap, CommentedSeq

def fix_tabs_in_file(file_path):
    """Replaces tabs with spaces in the YAML file."""
    with open(file_path, 'r', encoding='utf-8') as f:
        content = f.read()
    if '\t' in content:
        print(f"[TAB FIX] {file_path}")
        content = content.replace('\t', '    ')
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(content)

def find_and_remove_duplicates_in_place(data, seen_ids):
    """Removes duplicates from object lists directly in the source structure."""
    duplicates = []

    if isinstance(data, CommentedSeq):
        indices_to_remove = []
        for idx, item in enumerate(data):
            if isinstance(item, CommentedMap) and 'type' in item and 'id' in item:
                identifier = (str(item['type']), str(item['id']))
                if identifier in seen_ids:
                    duplicates.append(identifier)
                    indices_to_remove.append(idx)
                else:
                    seen_ids.add(identifier)
            else:
                dups = find_and_remove_duplicates_in_place(item, seen_ids)
                duplicates.extend(dups)

        for index in reversed(indices_to_remove):
            del data[index]

    elif isinstance(data, CommentedMap):
        for key, value in data.items():
            dups = find_and_remove_duplicates_in_place(value, seen_ids)
            duplicates.extend(dups)

    return duplicates

def should_skip_file(data):
    """Skip empty files or files without the required fields."""
    if data is None:
        return True
    if isinstance(data, list):
        return all(not isinstance(x, dict) or 'type' not in x or 'id' not in x for x in data)
    if isinstance(data, dict):
        return 'type' not in data or 'id' not in data
    return True

def is_empty_yaml(data):
    if data is None:
        return True
    if isinstance(data, CommentedSeq) and not data:
        return True
    if isinstance(data, CommentedMap) and not data:
        return True
    return False

def process_yaml_file(file_path, seen_ids):
    fix_tabs_in_file(file_path)

    yaml = YAML()
    yaml.preserve_quotes = True
    yaml.indent(mapping=2, sequence=2, offset=0)
    yaml.width = 4096

    try:
        with open(file_path, 'r', encoding='utf-8') as f:
            data = yaml.load(f)
    except Exception as e:
        print(f"[ERROR] Failed to parse {file_path}: {e}")
        return []

    if should_skip_file(data):
        print(f"[SKIP] {file_path} (nothing to process)")
        return []

    original_buf = io.StringIO()
    yaml.dump(data, original_buf)
    original_dump = original_buf.getvalue()

    duplicates = find_and_remove_duplicates_in_place(data, seen_ids)

    if not duplicates:
        return []
    
    if is_empty_yaml(data):
        print(f"[DELETE] {file_path} (file is now empty)")
        os.remove(file_path)
        return duplicates

    new_buf = io.StringIO()
    yaml.dump(data, new_buf)
    new_dump = new_buf.getvalue()

    if new_dump != original_dump:
        with open(file_path, 'w', encoding='utf-8') as f:
            f.write(new_dump)

    return duplicates

def process_directory(directory):
    seen_ids = set()
    total_duplicates = 0

    for root, _, files in os.walk(directory):
        for file in files:
            if file.endswith(('.yml', '.yaml')):
                file_path = os.path.join(root, file)
                try:
                    dups = process_yaml_file(file_path, seen_ids)
                    if dups:
                        print(f"[DUPLICATES] {file_path}:")
                        for t, i in dups:
                            print(f"  - Duplicate {t} ID '{i}'")
                        total_duplicates += len(dups)
                except Exception as e:
                    print(f"[ERROR] {file_path}: {e}")

    print(f"\n[SUMMARY] Total duplicates removed: {total_duplicates}")

if __name__ == '__main__':
    parser = argparse.ArgumentParser(description='Process YAML files in a directory.')
    parser.add_argument('directory', help='Path to the directory with YAML files')

    args = parser.parse_args()
    process_directory(args.directory)