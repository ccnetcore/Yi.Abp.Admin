export type ResultList = {
  status: number;
  data?: Array<any>;
};

export type Result = {
  status: number;
  data?: any;
};

export type ResultPage = {
  status: number;
  data?: {
    /** 列表数据 */
    items: Array<any>;
    /** 总条目数 */
    totalCount?: number;
  };
};

export type ResultFile = {
  status: number;
  data?: Array<{
    id: string;
  }>;
};
