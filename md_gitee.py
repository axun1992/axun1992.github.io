import sys
import re
import requests
import base64
import os
from pathlib import Path

def main():
    if len(sys.argv) < 2:
        print('[error]请输入md文件路径')
        return

    root = os.getcwd()
    root = root[0].lower() + root[1:]
    gitee = "gitee.com/yangxun666/yangxun983323204.github.io/raw/master/"
    md_path = sys.argv[1]
    md_path = md_path[0].lower() + md_path[1:]
    dir = os.path.dirname(md_path)
    name = Path(md_path).stem
    with open(md_path, 'r', encoding='utf-8') as f:
        str = f.read()

    img_path = re.findall('!\[.*?\]\((.*?)\)', str)

    for path in img_path:
        if re.match('http.://.*', path):
            continue
        elif not path.startswith('.'):
            continue

        fullPath = os.path.join(dir,path)
        rPath = fullPath.replace(root,"")
        giteeUrl = "https://" + (gitee+rPath).replace("\\","/").replace("//","/")
        str = str.replace('(%s)' % path, '(%s)' % giteeUrl)
        print('[替换文件:]{0}->{1}'.format(path,giteeUrl))
    
    out_path = os.path.join(dir,name+"_gitee.md")
    print("输出文件为:"+out_path)
    with open(out_path, 'w+', encoding='utf-8') as f:
        f.write(str)


main()
