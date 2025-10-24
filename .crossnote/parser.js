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