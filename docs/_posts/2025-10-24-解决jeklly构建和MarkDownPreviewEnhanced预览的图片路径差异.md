---
title:  "解决jeklly构建和MarkDownPreviewEnhanced预览的图片路径差异"
tags: markdown
---
在jeklly构建后的页面中，图片以根目录为相对目录，  
而在用MarkDownPreviewEnhanced插件本地直接预览md文件时，是以md所在目录为相对目录，  
这就造成一个问题：为了构建后的网页图片显示正确，在预览md时就无法显示图片。  
<!--more-->

显然jeklly那边不宜改动，因为我们最终目的还是要构建成网页显示。不过我们也同时希望拥有实时的本地md预览来查看编写的文档内容。  

这其实可以通过对MarkDownPreviewEnhanced适当设置来协调两者，那就是使用其“扩展解析器”的功能。  
## 解决方法，以windows+vscode为例：  
1. ctrl+shift+p调出命令框，执行Markdown Preview Enhanced: Extend Parser (Workspace)，将会在根目录创建.crossnote文件夹。  
2. 打开.crossnote文件夹中的parser.js文件，修改其onWillParseMarkdown函数：  

```js
({
  // Please visit the URL below for more information:
  // https://shd101wyy.github.io/markdown-preview-enhanced/#/extend-parser

  onWillParseMarkdown: async function (markdown) {
    return markdown.replace(/(\()(\/assets\/)([^"'\)]+\))/gm, '$1../assets/$3');
  },

  onDidParseMarkdown: async function (html) {
    return html;
  },
})
```   

3. 重启vscode后，应该在预览md时可以正常显示图片了。

![测试图片](/assets/img/ogp.png)

## 详细说明：  
在上面的方法中，重写了MarkDownPreviewEnhanced的解析器预处理阶段函数，把md文本中的图片路径修改为以md所在目录为相对目录的形式，使得预览时可以找到图片资产。  
正则表达式：`/(\()(\/assets)(\/[^"'\)]+\))/gm`匹配形如`(/assets/img/ogp.png)`这样的路径，并把其分为三个捕获组：  
- `(`
- `/assets/`
- `img/ogp.png)`  

把第二个捕获组由`/assets/` 替换为 `../assets/`，再把它放在捕获组一、三的中间，形成新的字符串以替换原匹配字符串。




