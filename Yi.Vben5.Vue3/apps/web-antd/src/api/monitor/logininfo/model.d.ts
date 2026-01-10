export interface LoginLog {
  id: string;
  loginUser: string;
  loginLocation: string;
  loginIp: string;
  browser: string;
  os: string;
  logMsg: string;
  creationTime: string;
  creatorId: string | null;
}
