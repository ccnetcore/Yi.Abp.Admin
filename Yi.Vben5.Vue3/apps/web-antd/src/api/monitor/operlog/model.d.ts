export interface OperationLog {
  id: string;
  title: string;
  operType: string;
  requestMethod: string;
  operUser: string;
  operIp: string;
  operLocation: string;
  method: string;
  requestParam: string;
  requestResult: string;
  creationTime: string;
  creatorId: string | null;
}
