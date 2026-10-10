// Monaco setup for the playground: the Mew language, an IL-style language for the LIR and
// IL views, the two editor themes, and the bridge that keeps the editor theme in step with
// the site's light/dark toggle. Loaded after Monaco and before Blazor, only on /playground.
(function () {
  // editor.main.js is an AMD bundle that fills in the `monaco` global only once its chunks
  // have loaded, so the language and theme setup waits on the loader. The theme bridge
  // below has no such dependency and is defined right away.
  require(['vs/editor/editor.main'], setup);

  function setup() {
  monaco.languages.register({ id: 'mew' });

  monaco.languages.setLanguageConfiguration('mew', {
    comments: { lineComment: '//', blockComment: ['/*', '*/'] },
    brackets: [['{', '}'], ['[', ']'], ['(', ')']],
    autoClosingPairs: [
      { open: '{', close: '}' },
      { open: '[', close: ']' },
      { open: '(', close: ')' },
      { open: '"', close: '"', notIn: ['string'] },
      { open: '\'', close: '\'', notIn: ['string', 'comment'] },
    ],
    surroundingPairs: [
      { open: '{', close: '}' },
      { open: '[', close: ']' },
      { open: '(', close: ')' },
      { open: '"', close: '"' },
    ],
  });

  // Token names follow Monaco's conventions so the standard themes also work; the mew
  // themes below colour the same names.
  monaco.languages.setMonarchTokensProvider('mew', {
    keywords: [
      'pub', 'static', 'fn', 'let', 'if', 'else', 'loop', 'while', 'break', 'continue',
      'return', 'as', 'is', 'type', 'namespace', 'use', 'for', 'in', 'field', 'mut',
      'external', 'internal', 'new', 'interface', 'impl', 'self', 'union', 'match',
      'task', 'rule', 'get', 'set', 'out',
    ],
    types: [
      'bool', 'i8', 'i16', 'i32', 'i64', 'u8', 'u16', 'u32', 'u64', 'f32', 'f64',
      'char', 'string', 'void', 'any', 'int',
    ],
    constants: ['true', 'false', 'null'],
    operators: [
      '->', '=>', '&&', '||', '==', '!=', '<=', '>=', '+=', '-=', '*=', '/=', '%=',
      '+', '-', '*', '/', '%', '=', '<', '>', '!', '?', '::', '...', '|',
    ],
    symbols: /[=><!~?:&|+\-*\/%.]+/,
    escapes: /\\(?:[0abfnrtv\\'"]|u[0-9A-Fa-f]{4})/,

    tokenizer: {
      root: [
        [/#[a-z_]+/, 'keyword.directive'],
        [/^\s*\[\s*[a-z_]\w*(\([^)]*\))?\s*\]/, 'annotation'],
        { include: '@whitespace' },
        [/\$"/, { token: 'string.quote', next: '@istring' }],
        [/"/, { token: 'string.quote', next: '@string' }],
        [/'(?:@escapes|[^'\\])'/, 'string.char'],
        [/0[xX][0-9a-fA-F_]+/, 'number.hex'],
        [/\d[\d_]*\.\d[\d_]*(?:[eE][+-]?\d[\d_]*)?/, 'number.float'],
        [/\d[\d_]*/, 'number'],
        [/[A-Z]\w*/, 'type.identifier'],
        [/[a-z_]\w*(?=\s*\()/, { cases: { '@keywords': 'keyword', '@types': 'type', '@default': 'function' } }],
        [/[a-z_]\w*/, { cases: { '@keywords': 'keyword', '@types': 'type', '@constants': 'constant', '@default': 'identifier' } }],
        [/[{}()\[\]]/, '@brackets'],
        [/@symbols/, { cases: { '@operators': 'operator', '@default': '' } }],
        [/[;,]/, 'delimiter'],
      ],

      whitespace: [
        [/\s+/, 'white'],
        [/\/\/\/.*$/, 'comment.doc'],
        [/\/\/.*$/, 'comment'],
        [/\/\*/, 'comment', '@comment'],
      ],

      comment: [
        [/[^/*]+/, 'comment'],
        [/\*\//, 'comment', '@pop'],
        [/[/*]/, 'comment'],
      ],

      string: [
        [/[^\\"]+/, 'string'],
        [/@escapes/, 'string.escape'],
        [/\\./, 'string.escape.invalid'],
        [/"/, { token: 'string.quote', next: '@pop' }],
      ],

      istring: [
        [/\{\{|\}\}/, 'string.escape'],
        [/\{/, { token: 'delimiter.interpolation', next: '@interpolation' }],
        [/[^\\"{}]+/, 'string'],
        [/@escapes/, 'string.escape'],
        [/\\./, 'string.escape.invalid'],
        [/"/, { token: 'string.quote', next: '@pop' }],
      ],

      interpolation: [
        [/\}/, { token: 'delimiter.interpolation', next: '@pop' }],
        { include: '@root' },
      ],
    },
  });

  // Shared by the LIR view (the compiler's own listing) and the IL view (ilspy's).
  monaco.languages.register({ id: 'msil' });
  monaco.languages.setMonarchTokensProvider('msil', {
    tokenizer: {
      root: [
        [/\/\/.*$/, 'comment'],
        [/^\s*[A-Za-z_]\w*:/, 'type.identifier'],
        [/\.[a-z]+/, 'keyword.directive'],
        [/\[[A-Za-z0-9_.]+\]/, 'annotation'],
        [/"(?:[^"\\]|\\.)*"/, 'string'],
        [/'(?:[^'\\]|\\.)*'/, 'string'],
        [/\b0x[0-9A-Fa-f]+\b/, 'number.hex'],
        [/\b\d+(?:\.\d+)?\b/, 'number'],
        [/^\s*(fn|main)\b/, 'keyword'],
        [/^\s*[a-z][a-z0-9.]*\b/, 'keyword'],
        [/\b(?:int32|int64|float32|float64|string|bool|object|void|class|static|public|private|hidebysig|cil|managed|extends|auto|ansi|abstract|sealed|beforefieldinit|init)\b/, 'type'],
        [/[A-Za-z_][\w.]*/, 'identifier'],
        [/[{}()\[\]]/, '@brackets'],
      ],
    },
  });

  function rules(c) {
    return [
      { token: 'keyword', foreground: c.keyword, fontStyle: 'bold' },
      { token: 'keyword.directive', foreground: c.directive },
      { token: 'type', foreground: c.type },
      { token: 'type.identifier', foreground: c.type },
      { token: 'function', foreground: c.fn },
      { token: 'identifier', foreground: c.text },
      { token: 'string', foreground: c.string },
      { token: 'string.quote', foreground: c.string },
      { token: 'string.char', foreground: c.string },
      { token: 'string.escape', foreground: c.escape },
      { token: 'delimiter.interpolation', foreground: c.escape, fontStyle: 'bold' },
      { token: 'number', foreground: c.number },
      { token: 'number.hex', foreground: c.number },
      { token: 'number.float', foreground: c.number },
      { token: 'constant', foreground: c.number },
      { token: 'comment', foreground: c.comment, fontStyle: 'italic' },
      { token: 'comment.doc', foreground: c.comment, fontStyle: 'italic' },
      { token: 'annotation', foreground: c.directive },
      { token: 'operator', foreground: c.text },
    ];
  }

  // Monaco wants literal colours, so the site's palette cannot be read from CSS variables
  // here; these are chosen to sit with it. The editor background stays transparent so the
  // host pane paints it in both themes.
  monaco.editor.defineTheme('mew-light', {
    base: 'vs',
    inherit: true,
    rules: rules({ keyword: '6d28d9', directive: '9333ea', type: '0f766e', fn: '1d4ed8', text: '1f2937', string: 'b45309', escape: 'c2410c', number: '0369a1', comment: '94a3b8' }),
    colors: { 'editor.background': '#00000000', 'editorGutter.background': '#00000000' },
  });
  monaco.editor.defineTheme('mew-dark', {
    base: 'vs-dark',
    inherit: true,
    rules: rules({ keyword: 'c4b5fd', directive: 'd8b4fe', type: '5eead4', fn: '93c5fd', text: 'e5e7eb', string: 'fcd34d', escape: 'fdba74', number: '7dd3fc', comment: '64748b' }),
    colors: { 'editor.background': '#00000000', 'editorGutter.background': '#00000000' },
  });
  }

  var observer = null;

  function current() {
    return document.documentElement.classList.contains('dark') ? 'dark' : 'light';
  }

  window.mewPlayground = {
    // Reports the theme now and calls back into the island on every flip of the site's
    // theme toggle, which stamps `.dark` on <html>.
    observeTheme: function (dotNetRef) {
      if (observer) {
        observer.disconnect();
      }
      var last = current();
      observer = new MutationObserver(function () {
        var now = current();
        if (now === last) {
          return;
        }
        last = now;
        dotNetRef.invokeMethodAsync('OnHostThemeChanged', now);
      });
      observer.observe(document.documentElement, { attributes: true, attributeFilter: ['class'] });
      return last;
    },
    ready: function () {
      document.documentElement.setAttribute('data-playground', 'ready');
    },
  };
})();
