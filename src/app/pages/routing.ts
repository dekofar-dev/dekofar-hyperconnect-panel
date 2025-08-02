import { Routes } from '@angular/router';

const Routing: Routes = [
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./dashboard/dashboard.module').then((m) => m.DashboardModule),
  },
  {
    path: 'builder',
    loadChildren: () =>
      import('./builder/builder.module').then((m) => m.BuilderModule),
  },
  {
    path: 'crafted/pages/profile',
    loadChildren: () =>
      import('../modules/profile/profile.module').then((m) => m.ProfileModule),
  },
  {
    path: 'crafted/account',
    loadChildren: () =>
      import('../modules/account/account.module').then((m) => m.AccountModule),
  },
  {
    path: 'crafted/pages/wizards',
    loadChildren: () =>
      import('../modules/wizards/wizards.module').then((m) => m.WizardsModule),
  },

  {
    path: 'crafted/widgets',
    loadChildren: () =>
      import('../modules/widgets-examples/widgets-examples.module').then(
        (m) => m.WidgetsExamplesModule
      ),
  },
  {
    path: 'apps/chat',
    loadChildren: () =>
      import('../modules/apps/chat/chat.module').then((m) => m.ChatModule),
  },

  // ✅ Özel sayfalar (pages klasörü altındaki feature modüller)

//   {
//     path: 'shipping',
//     loadChildren: () =>
//       import('../pages/shipping/shipping.module').then((m) => m.ShippingModule),
//   },

//   {
//     path: 'returns',
//     loadChildren: () =>
//       import('../pages/returns/returns.module').then((m) => m.ReturnsModule),
//   },
//   {
//     path: 'finance',
//     loadChildren: () =>
//       import('../pages/finance/finance.module').then((m) => m.FinanceModule),
//   },
//   {
//     path: 'inventory',
//     loadChildren: () =>
//       import('../pages/inventory/inventory.module').then((m) => m.InventoryModule),
//   },
//   {
//     path: 'mailbox',
//     loadChildren: () =>
//       import('../pages/mailbox/mailbox.module').then((m) => m.MailboxModule),
//   },
  {
    path: 'orders',
    loadChildren: () =>
      import('./orders/orders.module').then((m) => m.OrdersPageModule),
  },
  {
    path: 'mailbox',
    loadChildren: () =>
      import('../modules/mailbox/mailbox.module').then((m) => m.MailboxModule),
  },
  {
    path: 'support-tickets',
    loadChildren: () =>
      import('../modules/support-tickets/support-tickets.module').then((m) => m.SupportTicketsModule),
  },
  {
    path: 'support-categories',
    loadChildren: () =>
      import('../modules/support-categories/support-categories.module').then((m) => m.SupportCategoriesModule),
  },
  {
    path: 'discounts',
    loadChildren: () =>
      import('../modules/discounts/discounts.module').then((m) => m.DiscountsModule),
  },
  {
    path: 'commissions',
    loadChildren: () =>
      import('../modules/commissions/commissions.module').then((m) => m.CommissionsModule),
  },
  {
    path: 'users',
    loadChildren: () =>
      import('../modules/users/users.module').then((m) => m.UsersModule),
  },
  {
    path: 'pin-settings',
    loadChildren: () =>
      import('../modules/pin-settings/pin-settings.module').then(
        (m) => m.PinSettingsModule
      ),
  },
  {
    path: '',
    redirectTo: '/dashboard',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'error/404',
  },
];

export { Routing };
