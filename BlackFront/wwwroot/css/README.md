# CSS refactor

The original `style.css` (994 lines) was split into focused modules.

## Files
- `base.css` — variables, reset, typography, global links
- `layout.css` — main, header, nav, sidebar, logout
- `components.css` — shared form controls, metadata dates, editorial badges, typewriter
- `pages/news.css` — news feed, featured posts, search
- `pages/post.css` — article, recommendations, next material
- `pages/admin.css` — admin forms/actions
- `pages/contacts.css` — contacts/social icons
- `pages/error.css` — error page
- `style.css` — import-only entry point

## Integration
You can keep the current Razor layout exactly as it is:

```html
<link rel="stylesheet" href="~/css/style.css">
```

`style.css` imports the modules in the correct order.

## Safe cleanup performed
- merged duplicated `.contact-container`, `.social-icons`, `.icon-box`, `.icon-box:hover` rules
- removed duplicated `.news-list` and `.news-post` rules
- preserved the existing global `a` button-like behavior to avoid visual regressions
- preserved `site.css` untouched

## Suggested later cleanup
The biggest architectural debt that remains is the global `a { ...button styles... }`. Later, move button appearance to a `.btn` class and make normal `a` elements ordinary links. Do that as a separate visual change, because it can affect many pages.
