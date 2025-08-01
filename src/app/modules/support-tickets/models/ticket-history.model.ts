// ticket-history.model.ts
export interface SupportTicketHistoryDto {
  id: number;
    status: number; // örn: 0 = Açık, 1 = İnceleme, vs.

  fieldChanged: string;
  oldValue: string | null;
  newValue: string;
  changedAt: string;
  changedBy: string;
}