import { reactive } from "vue";
import type { FormRules } from "element-plus";

/** 自定义表单规则校验 */
export const formRules = reactive(<FormRules>{
  configName: [
    { required: true, message: "参数名称为必填项", trigger: "blur" }
  ],
  configKey: [{ required: true, message: "参数键名为必填项", trigger: "blur" }],
  configValue: [
    { required: true, message: "参数键值为必填项", trigger: "blur" }
  ]
});
