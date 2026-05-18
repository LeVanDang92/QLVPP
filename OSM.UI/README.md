# OSMUI

This project was generated using [Angular CLI](https://github.com/angular/angular-cli) version 21.2.6.

## Development server

To start a local development server, run:

```bash
ng serve
```

Once the server is running, open your browser and navigate to `http://localhost:4200/`. The application will automatically reload whenever you modify any of the source files.

## Code scaffolding

Angular CLI includes powerful code scaffolding tools. To generate a new component, run:

```bash
ng generate component component-name
```

For a complete list of available schematics (such as `components`, `directives`, or `pipes`), run:

```bash
ng generate --help
```

## Building

To build the project run:

```bash
ng build
```

This will compile your project and store the build artifacts in the `dist/` directory. By default, the production build optimizes your application for performance and speed.

## Running unit tests

To execute unit tests with the [Vitest](https://vitest.dev/) test runner, use the following command:

```bash
ng test
```

## Running end-to-end tests

For end-to-end (e2e) testing, run:

```bash
ng e2e
```

Angular CLI does not come with an end-to-end testing framework by default. You can choose one that suits your needs.

## Additional Resources

For more information on using the Angular CLI, including detailed command references, visit the [Angular CLI Overview and Command Reference](https://angular.dev/tools/cli) page.

=================================

# OSM SmartHR Angular Template

Angular 21.2.6 template converted from the SmartHR HTML demo.

## Main improvements in this version

- Horizontal Single layout is still the default layout.
- The large static menu HTML was moved into TypeScript data.
- Header menu was separated into smaller components.
- Mobile sidebar also uses the same menu data, so desktop/mobile menu stay consistent.
- Workspace tabs are saved in `localStorage`, so refreshing the browser keeps opened tabs.
- Layout switcher is removed. The project only keeps the Horizontal Single style.

## Run

```bash
npm install
npm start
```

Open:

```text
http://localhost:4200
```

## Important folder structure

```text
src/app
├── app.routes.ts
├── core/layout
│   ├── header
│   │   └── header.component.*
│   ├── main-layout
│   │   └── main-layout.component.*
│   ├── mobile-sidebar
│   │   └── mobile-sidebar.component.*
│   ├── menu
│   │   ├── menu-item.model.ts
│   │   ├── menu.data.ts
│   │   ├── horizontal-menu
│   │   │   └── horizontal-menu.component.*
│   │   └── mobile-menu
│   │       └── mobile-menu.component.*
│   └── tabs
│       ├── page-tab.service.ts
│       └── page-tabs.component.*
└── features
    ├── dashboard
    ├── operation
    ├── product-mix-operation-capa
    └── workspace-page
```

## How the menu works

Menu data is defined in:

```text
src/app/core/layout/menu/menu.data.ts
```

Desktop horizontal menu uses:

```text
src/app/core/layout/menu/horizontal-menu/horizontal-menu.component.*
```

Mobile sidebar menu uses:

```text
src/app/core/layout/menu/mobile-menu/mobile-menu.component.*
```

Both components read from the same `APP_MENU_SECTIONS`, so you only need to edit the menu in one place.

## How tabs work

Tabs are managed by:

```text
src/app/core/layout/tabs/page-tab.service.ts
```

The service stores opened tabs in:

```text
localStorage key: osm.page-tabs.v1
```

This means when the user refreshes the page, opened tabs are restored automatically.

## Add a new real page

Example: add Inventory page.

### 1. Create component

```text
src/app/features/inventory/inventory.component.ts
src/app/features/inventory/inventory.component.html
src/app/features/inventory/inventory.component.scss
```

### 2. Add route

In `src/app/app.routes.ts`:

```ts
{
  path: 'inventory',
  loadComponent: () =>
    import('./features/inventory/inventory.component')
      .then((m) => m.InventoryComponent),
}
```

### 3. Add menu item

In `src/app/core/layout/menu/menu.data.ts`:

```ts
page('inventory', 'Inventory', undefined, '/inventory')
```

After that, clicking Inventory opens a new tab and shows the Inventory page.

## Temporary pages

If a menu item does not have a real route, it can still open using:

```text
/workspace/<slug>
```

The placeholder page is:

```text
src/app/features/workspace-page
```

