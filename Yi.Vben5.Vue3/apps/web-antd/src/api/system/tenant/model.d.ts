export interface Tenant {
  id: string;
  name: string;
  entityVersion: number;
  tenantConnectionString: string;
  dbType: number;
  isDeleted: boolean;
  creationTime: string;
  creatorId: string | null;
  lastModifierId: string | null;
  lastModificationTime: string | null;
  // 以下字段可能来自 ExtraProperties 或后端扩展
  accountCount?: number;
  address?: string;
  companyName?: string;
  contactPhone?: string;
  contactUserName?: string;
  domain?: string;
  expireTime?: string;
  intro?: string;
  licenseNumber?: any;
  packageId?: string;
  remark?: string;
  status?: string | boolean;
  tenantId?: string;
}
