import { MenuItem, MenuSection } from './menu-item.model';

export const DASHBOARD_TAB_PATH = '/dashboard';

export function toWorkspacePath(title: string): string {
  const slug = title
    .toLowerCase()
    .replace(/&/g, 'and')
    .replace(/[^a-z0-9]+/g, '-')
    .replace(/^-+|-+$/g, '')
    .slice(0, 80);

  return `/workspace/${slug || 'page'}`;
}

function page(id: string, title: string, icon?: string, path?: string, children?: MenuItem[]): MenuItem {
  return {
    id,
    title,
    icon,
    path: path ?? toWorkspacePath(title),
    closable: path !== DASHBOARD_TAB_PATH,
    children,
  };
}

function group(id: string, title: string, icon: string | undefined, children: MenuItem[], badgeText?: string): MenuItem {
  return {
    id,
    title,
    icon,
    badge: badgeText ? { text: badgeText, className: 'badge badge-danger fs-10 fw-medium text-white p-1' } : undefined,
    children,
  };
}

export const APP_MENU_SECTIONS: MenuSection[] = [
  {
    title: 'Main',
    items: [
      group('dashboard', 'Dashboard', 'ti ti-smart-home', [
        page('admin-dashboard', 'Admin Dashboard', undefined, DASHBOARD_TAB_PATH),
        page('operation', 'Operation', undefined, '/operation'),
        page('product-mix-operation-capa', 'Product Mix(Operation Capa)', undefined, '/product-mix-operation-capa'),
        page('employee-dashboard', 'Employee Dashboard'),
        page('deals-dashboard', 'Deals Dashboard'),
        page('leads-dashboard', 'Leads Dashboard'),
        page('hr-dashboard', 'HR Dashboard'),
        page('payroll-dashboard', 'Payroll Dashboard'),
        page('recruitment-dashboard', 'Recruitment Dashboard'),
        page('attendance-dashboard', 'Attendance Dashboard'),
        page('finance-dashboard', 'Finance Dashboard'),
        page('it-admin-dashboard', 'IT Admin Dashboard'),
        page('asset-dashboard', 'Asset Dashboard'),
        page('help-desk-dashboard', 'Help Desk Dashboard'),
      ], 'Hot'),
      group('super-admin', 'Super Admin', 'ti ti-user-star', [
        page('super-dashboard', 'Dashboard'),
        page('companies', 'Companies'),
        page('subscriptions', 'Subscriptions'),
        page('packages', 'Packages'),
        page('domain', 'Domain'),
        page('purchase-transaction', 'Purchase Transaction'),
        page('tenant-usage-metrics', 'Tenant Usage Metrics'),
        page('tenant-support-tickets', 'Tenant Support Tickets'),
        group('tickets-admin', 'Tickets', undefined, [
          page('agents', 'Agents'),
          page('sla-policies', 'SLA Policies'),
          page('escalation-rules', 'Escalation Rules'),
        ]),
      ]),
      group('applications', 'Applications', 'ti ti-layout-grid-add', [
        page('chat', 'Chat'),
        group('calls', 'Calls', undefined, [
          page('voice-call', 'Voice Call'),
          page('video-call', 'Video Call'),
          page('outgoing-call', 'Outgoing Call'),
          page('incoming-call', 'Incoming Call'),
          page('call-history', 'Call History'),
        ]),
        page('calendar', 'Calendar'),
        page('email', 'Email'),
        page('todo', 'To Do'),
        page('notes', 'Notes'),
        page('file-manager', 'File Manager'),
        page('kanban', 'Kanban'),
        page('invoices', 'Invoices'),
      ]),
    ],
  },
  {
    title: 'Projects',
    items: [
      group('projects-root', 'Projects', 'ti ti-user-star', [
        page('clients', 'Clients'),
        group('project-pages', 'Projects', undefined, [
          page('projects', 'Projects'),
          page('tasks', 'Tasks'),
          page('task-board', 'Task Board'),
        ]),
        group('crm-projects', 'CRM', undefined, [
          page('contacts', 'Contacts'),
          page('companies-crm', 'Companies'),
          page('deals', 'Deals'),
          page('leads', 'Leads'),
          page('pipeline', 'Pipeline'),
          page('analytics', 'Analytics'),
          page('activities', 'Activities'),
        ]),
        group('hrm-projects', 'HRM', undefined, [
          group('employees', 'Employees', undefined, [
            page('employee-lists', 'Employee Lists'),
            page('employee-grid', 'Employee Grid'),
            page('employee-details', 'Employee Details'),
            page('departments', 'Departments'),
            page('designations', 'Designations'),
            page('policies', 'Policies'),
          ]),
        ]),
        group('tickets', 'Tickets', undefined, [
          page('tickets', 'Tickets'),
          page('ticket-details', 'Ticket Details'),
          page('ticket-automation', 'Ticket Automation'),
          page('ticket-reports', 'Ticket Reports'),
        ]),
        page('holidays', 'Holidays'),
        group('attendance', 'Attendance', undefined, [
          group('leaves', 'Leaves', undefined, [
            page('leaves-admin', 'Leaves (Admin)'),
            page('leave-employee', 'Leave (Employee)'),
            page('leave-settings', 'Leave Settings'),
          ]),
          page('attendance-admin', 'Attendance (Admin)'),
          page('attendance-employee', 'Attendance (Employee)'),
          page('timesheets', 'Timesheets'),
          page('shift-schedule', 'Shift & Schedule'),
          page('shift-swap-requests', 'Shift Swap Requests'),
          page('overtime', 'Overtime'),
          page('holiday-calendar', 'Holiday Calendar'),
          page('wfh-management', 'WFH Management'),
        ]),
        group('performance', 'Performance', undefined, [
          page('performance-indicator', 'Performance Indicator'),
          page('performance-review', 'Performance Review'),
          page('performance-appraisal', 'Performance Appraisal'),
          page('goal-list', 'Goal List'),
          page('goal-type', 'Goal Type'),
        ]),
        group('training', 'Training', undefined, [
          page('training-list', 'Training List'),
          page('trainers', 'Trainers'),
          page('training-type', 'Training Type'),
          page('certification-tracking', 'Certification Tracking'),
          page('learning-analytics', 'Learning Analytics'),
        ]),
        page('probation-management', 'Probation Management'),
        page('notice-period-tracker', 'Notice Period Tracker'),
        page('promotion', 'Promotion'),
        page('resignation', 'Resignation'),
        page('termination', 'Termination'),
        group('recruitment', 'Recruitment', undefined, [
          page('jobs', 'Jobs'),
          page('candidates', 'Candidates'),
          page('referrals', 'Referrals'),
          page('resume-parsing', 'Resume Parsing'),
          page('campus-hiring', 'Campus Hiring'),
        ]),
      ]),
    ],
  },
  {
    title: 'Administration',
    items: [
      group('administration', 'Administration', 'ti ti-user-star', [
        group('sales', 'Sales', undefined, [
          page('estimates', 'Estimates'),
          page('sales-invoices', 'Invoices'),
          page('payments', 'Payments'),
          page('expenses', 'Expenses'),
          page('provident-fund', 'Provident Fund'),
          page('taxes', 'Taxes'),
        ]),
        group('accounting', 'Accounting', undefined, [
          page('categories', 'Categories'),
          page('budgets', 'Budgets'),
          page('budget-expenses', 'Budget Expenses'),
          page('budget-revenues', 'Budget Revenues'),
        ]),
        group('payroll', 'Payroll', undefined, [
          page('employee-salary', 'Employee Salary'),
          page('payslip', 'Payslip'),
          page('payroll-items', 'Payroll Items'),
        ]),
        group('assets', 'Assets', undefined, [
          page('assets-list', 'Assets'),
          page('asset-categories', 'Asset Categories'),
        ]),
        group('help-supports', 'Help & Supports', undefined, [
          page('knowledge-base', 'Knowledge Base'),
          page('support-activities', 'Activities'),
        ]),
        group('user-management', 'User Management', undefined, [
          page('users', 'Users'),
          page('roles-permissions', 'Roles & Permissions'),
        ]),
        group('reports', 'Reports', undefined, [
          page('expense-report', 'Expense Report'),
          page('invoice-report', 'Invoice Report'),
          page('payment-report', 'Payment Report'),
          page('project-report', 'Project Report'),
          page('task-report', 'Task Report'),
          page('user-report', 'User Report'),
          page('employee-report', 'Employee Report'),
          page('payslip-report', 'Payslip Report'),
          page('attendance-report', 'Attendance Report'),
          page('leave-report', 'Leave Report'),
          page('daily-report', 'Daily Report'),
        ]),
        group('settings', 'Settings', undefined, [
          group('general-settings', 'General Settings', undefined, [
            page('profile', 'Profile'),
            page('security', 'Security'),
            page('notifications', 'Notifications'),
            page('connected-apps', 'Connected Apps'),
          ]),
          group('website-settings', 'Website Settings', undefined, [
            page('business-settings', 'Business Settings'),
            page('seo-settings', 'SEO Settings'),
            page('localization', 'Localization'),
            page('prefixes', 'Prefixes'),
            page('preferences', 'Preferences'),
            page('appearance', 'Appearance'),
            page('language', 'Language'),
            page('authentication-settings', 'Authentication'),
            page('ai-settings', 'AI Settings'),
          ]),
          group('app-settings', 'App Settings', undefined, [
            page('salary-settings', 'Salary Settings'),
            page('approval-settings', 'Approval Settings'),
            page('invoice-settings', 'Invoice Settings'),
            page('leave-type', 'Leave Type'),
            page('custom-fields', 'Custom Fields'),
          ]),
          group('system-settings', 'System Settings', undefined, [
            page('email-settings', 'Email Settings'),
            page('email-templates', 'Email Templates'),
            page('sms-settings', 'SMS Settings'),
            page('sms-templates', 'SMS Templates'),
            page('otp', 'OTP'),
            page('gdpr-cookies', 'GDPR Cookies'),
            page('maintenance-mode', 'Maintenance Mode'),
          ]),
          group('financial-settings', 'Financial Settings', undefined, [
            page('payment-gateways', 'Payment Gateways'),
            page('tax-rate', 'Tax Rate'),
            page('currencies', 'Currencies'),
          ]),
          group('other-settings', 'Other Settings', undefined, [
            page('custom-css', 'Custom CSS'),
            page('custom-js', 'Custom JS'),
            page('cronjob', 'Cronjob'),
            page('storage', 'Storage'),
            page('ban-ip-address', 'Ban IP Address'),
            page('backup', 'Backup'),
            page('clear-cache', 'Clear Cache'),
          ]),
        ]),
      ]),
    ],
  },
  {
    title: 'Pages',
    items: [
      group('pages', 'Pages', 'ti ti-page-break', [
        page('starter', 'Starter'),
        page('profile-page', 'Profile'),
        page('profile-settings', 'Profile Settings'),
        page('gallery', 'Gallery'),
        page('search-results', 'Search Results'),
        page('timeline', 'Timeline'),
        page('pricing', 'Pricing'),
        page('coming-soon', 'Coming Soon'),
        page('under-maintenance', 'Under Maintenance'),
        page('under-construction', 'Under Construction'),
        page('api-keys', 'API Keys'),
        page('privacy-policy', 'Privacy Policy'),
        page('terms-conditions', 'Terms & Conditions'),
      ]),
    ],
  },
  {
    title: 'Extras',
    items: [
      group('extras', 'Extras', 'ti ti-vector-triangle', [
        group('content', 'Content', undefined, [
          page('content-pages', 'Pages'),
          group('blogs', 'Blogs', undefined, [
            page('all-blogs', 'All Blogs'),
            page('blog-categories', 'Categories'),
            page('blog-comments', 'Comments'),
            page('blog-tags', 'Tags'),
          ]),
          group('locations', 'Locations', undefined, [
            page('countries', 'Countries'),
            page('states', 'States'),
            page('cities', 'Cities'),
          ]),
          page('testimonials', 'Testimonials'),
          page('faq', 'FAQ’S'),
        ]),
        group('authentication', 'Authentication', undefined, [
          group('login', 'Login', undefined, [page('login-cover', 'Cover'), page('login-illustration', 'Illustration'), page('login-basic', 'Basic')]),
          group('register', 'Register', undefined, [page('register-cover', 'Cover'), page('register-illustration', 'Illustration'), page('register-basic', 'Basic')]),
          group('forgot-password', 'Forgot Password', undefined, [page('forgot-cover', 'Cover'), page('forgot-illustration', 'Illustration'), page('forgot-basic', 'Basic')]),
          group('reset-password', 'Reset Password', undefined, [page('reset-cover', 'Cover'), page('reset-illustration', 'Illustration'), page('reset-basic', 'Basic')]),
          page('lock-screen', 'Lock Screen'),
          page('error-404', '404 Error'),
          page('error-500', '500 Error'),
        ]),
        group('ui-interface', 'UI Interface', undefined, [
          group('base-ui', 'Base UI', undefined, [
            page('alerts', 'Alerts'), page('accordion', 'Accordion'), page('avatar', 'Avatar'), page('badges', 'Badges'),
            page('breadcrumb', 'Breadcrumb'), page('buttons', 'Buttons'), page('button-group', 'Button Group'),
            page('cards', 'Card'), page('carousel', 'Carousel'), page('collapse', 'Collapse'), page('dropdowns', 'Dropdowns'),
            page('grid', 'Grid'), page('modals', 'Modals'), page('tabs', 'Tabs'), page('toasts', 'Toasts'), page('tooltips', 'Tooltips'), page('typography', 'Typography'),
          ]),
          group('forms', 'Forms', undefined, [
            group('form-elements', 'Form Elements', undefined, [page('basic-inputs', 'Basic Inputs'), page('checkbox-radios', 'Checkbox & Radios'), page('input-groups', 'Input Groups'), page('file-uploads', 'File Uploads')]),
            page('form-validation', 'Form Validation'),
            page('select2', 'Select2'),
            page('form-wizard', 'Form Wizard'),
          ]),
          group('tables', 'Tables', undefined, [page('basic-tables', 'Basic Tables'), page('data-table', 'Data Table')]),
          group('charts', 'Charts', undefined, [page('apex-charts', 'Apex Charts'), page('chart-js', 'Chart Js'), page('morris-charts', 'Morris Charts')]),
          group('icons', 'Icons', undefined, [page('fontawesome-icons', 'Fontawesome Icons'), page('tabler-icons', 'Tabler Icons'), page('bootstrap-icons', 'Bootstrap Icons')]),
        ]),
        {
          id: 'documentation',
          title: 'Documentation',
          externalUrl: 'https://smarthr.co.in/demo/documentation/html',
        },
        page('change-log', 'Change Log'),
        group('multi-level', 'Multi Level', undefined, [
          page('multilevel-1', 'Multilevel 1'),
          group('multilevel-2', 'Multilevel 2', undefined, [
            page('multilevel-2-1', 'Multilevel 2.1'),
            group('multilevel-2-2', 'Multilevel 2.2', undefined, [page('multilevel-2-2-1', 'Multilevel 2.2.1'), page('multilevel-2-2-2', 'Multilevel 2.2.2')]),
          ]),
          page('multilevel-3', 'Multilevel 3'),
        ]),
      ]),
    ],
  },
];

export function flattenMenuItems(sections: MenuSection[] = APP_MENU_SECTIONS): MenuItem[] {
  const result: MenuItem[] = [];

  const walk = (items: MenuItem[]) => {
    for (const item of items) {
      result.push(item);
      if (item.children?.length) {
        walk(item.children);
      }
    }
  };

  for (const section of sections) {
    walk(section.items);
  }

  return result;
}

export function findMenuItemByPath(path: string): MenuItem | undefined {
  return flattenMenuItems().find((item) => item.path === path);
}

export function titleFromPath(path: string): string {
  const found = findMenuItemByPath(path);

  if (found) {
    return found.title === 'Admin Dashboard' ? 'Dashboard' : found.title;
  }

  const slug = path.split('/').filter(Boolean).pop() ?? 'page';

  return slug
    .split('-')
    .filter(Boolean)
    .map((word) => word.charAt(0).toUpperCase() + word.slice(1))
    .join(' ');
}
