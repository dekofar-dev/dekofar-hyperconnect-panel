## Advanced Modular Backend Development for E-commerce Admin Panel API (ASP.NET Core)

This document consolidates the backend requirements for the modular e-commerce administration system. It covers order management, customer service, logistics, communication and operational tooling. The system integrates Shopify and manual orders while supporting multiple shipping companies and notification channels.

### Core Objectives
- Provide a robust, extensible API suitable for a modular Angular admin panel.
- Support secure access with role-based authorization and configurable system settings.

### Key Modules
1. **Order Tagging System**
   - `OrderTag` entity with name, color and active flag.
   - `OrderOrderTag` junction table for many-to-many relations.
   - Tagging applies to Shopify and manual orders with filtering via `GET /api/orders?tags=Pending,Kargoda`.

2. **Manual Call Request System**
   - `CallRequest` model with customer details, subject, description and status (`Pending`, `Completed`, `Rejected`).
   - Supports admin assignment and status updates; optional NetGSM call log integration.

3. **Shipping & Cargo Tracking**
   - `CargoCompany` table (name, API URL, logo) and order fields for company, tracking number and status.
   - Allows manual tracking number entry and optional API integration with Turkish cargo providers.

4. **Invoice Upload & Management**
   - `InvoiceFile` model linking orders to stored PDF invoices.
   - Includes upload, preview and download with secure storage.

5. **Notification System**
   - `Notification` entity containing message, user, read timestamp and created timestamp.
   - Real-time delivery via SignalR with read/unread state management.

6. **System-wide Announcement Broadcast**
   - `Announcement` model with title, content, expiry and activation flag.
   - Visible on dashboard, supports scheduling and optional role targeting.

7. **Excel / CSV Import for Manual Orders**
   - Bulk manual order creation from XLSX/CSV files.
   - Requires field mapping, upload history and validation feedback.

8. **Email Integration (SMTP / Mailjet)**
   - Configurable SMTP settings stored in `SystemSettings`.
   - Template-based emails for returns, call requests and order notifications.

9. **WhatsApp Automation via NetGSM**
   - Automated messages for order shipment, return outcomes and call request updates.
   - Template messages stored in the database with phone number validation.

10. **Global Search API**
    - `GET /api/search?q=keyword` endpoint searching across users, orders, tickets and returns.

11. **Return Enhancements**
    - Extends existing return workflow with overdue detection, detailed statuses, tracking numbers and monthly analytics.

12. **Audit Logging & Action History**
    - `AuditLog` table capturing entity, action, user and change details for traceability.

### Roles & Access
- **Admin:** full access to all endpoints.
- **Support:** read-only access to returns, tickets and orders.
- **User:** limited to managing own data, returns and call requests.

### Best Practices
- JWT authentication with role-based `[Authorize]` attributes.
- Centralized `SystemSettings` for configurable values.
- File type/size validation (`.jpg`, `.png`, `.pdf`, max 10MB).
- Pagination and filtering on listing endpoints.
- Structured Swagger documentation and logging middleware.
- Unit tests for core services.

