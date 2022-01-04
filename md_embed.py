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

    md_path = sys.argv[1]
    dir = os.path.dirname(md_path)
    name = Path(md_path).stem
    with open(md_path, 'r', encoding='utf-8') as f:
        str = f.read()
    str += '\n\n\n\n\n\n'
    img_path = re.findall('!\[.*?\]\((.*?)\)', str)
    key = 0
    for path in img_path:
        if re.match('http.://.*', path):
            try:
                data = requests.get(url=path).content
            except Exception:
                print('[warn]请检查网络连接或图片链接(%s)是否有效' % path)
                continue
        else:
            try:
                fullPath = path
                if path.startswith('.'):
                    fullPath = os.path.join(dir,path)

                with open(fullPath, 'rb') as f:
                    data = f.read()
            except FileNotFoundError:
                print('[warn]请检查本地文件(%s)是否存在' % path)
                continue
        print('[success]%s' % path)
        base64_str = base64.b64encode(data).decode()
        str = str.replace('(%s)' % path, '[%d]' % key)
        str += '[%d]:data:image/jpg;base64,%s\n\n' % (key, base64_str)
        key += 1
    
    out_path = os.path.join(dir,name+"_embed.md")
    print("输出文件为:"+out_path)
    with open(out_path, 'w+', encoding='utf-8') as f:
        f.write(str)


main()
