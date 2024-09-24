import { reactive } from "vue";
import type { FormRules } from "element-plus";

/** 自定义表单规则校验 */
export const formRules = reactive(<FormRules>{
  postName: [{ required: true, message: "岗位名称为必填项", trigger: "blur" }],
  postCode: [{ required: true, message: "岗位标识为必填项", trigger: "blur" }]
});
