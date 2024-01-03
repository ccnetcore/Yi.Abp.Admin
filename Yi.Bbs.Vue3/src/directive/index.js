import hasRole from './permission/hasRole'
import hasPermi from './permission/hasPermi'
// import copyText from './common/copyText'

export default function directive(app){
  app.directive('hasRole', hasRole)
  app.directive('hasPer', hasPermi)
//   app.directive('copyText', copyText)
}