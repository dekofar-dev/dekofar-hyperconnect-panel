export interface WorkSession {
  id: string;
  userId: string;
  user?: any;
  startTime: string;
  endTime?: string | null;
  startIp?: string | null;
  duration?: string | null;
}
