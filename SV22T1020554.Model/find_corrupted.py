import os
import re

def find_corrupted_summaries(root_dir):
    corrupted_files = {}
    
    # Common Vietnamese characters that are often corrupted or replaced by '?'
    # This regex looks for '///' followed by anything containing '?' or replacement characters
    # Or non-ascii characters that might be part of the corruption
    pattern = re.compile(r'///.*[?\ufffd]')
    
    for subdir, dirs, files in os.walk(root_dir):
        if 'bin' in dirs: dirs.remove('bin')
        if 'obj' in dirs: dirs.remove('obj')
        
        for file in files:
            if file.endswith('.cs'):
                filepath = os.path.join(subdir, file)
                try:
                    with open(filepath, 'r', encoding='utf-8', errors='replace') as f:
                        lines = f.readlines()
                        for i, line in enumerate(lines):
                            if '///' in line and ('?' in line or '\ufffd' in line):
                                if filepath not in corrupted_files:
                                    corrupted_files[filepath] = []
                                corrupted_files[filepath].append({
                                    'line_number': i + 1,
                                    'content': line.strip()
                                })
                except Exception as e:
                    print(f"Error reading {filepath}: {e}")
                    
    return corrupted_files

if __name__ == "__main__":
    root = r'd:\SV22T1020554\SV22T1020554.Model'
    results = find_corrupted_summaries(root)
    
    with open('corrupted_report.txt', 'w', encoding='utf-8') as f:
        for path, matches in results.items():
            f.write(f"File: {path}\n")
            for m in matches:
                f.write(f"  {m['line_number']}: {m['content']}\n")
            f.write("\n")
    print(f"Report generated in corrupted_report.txt with {len(results)} files.")
