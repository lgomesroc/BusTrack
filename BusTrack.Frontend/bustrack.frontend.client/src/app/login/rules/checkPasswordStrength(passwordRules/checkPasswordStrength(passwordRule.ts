import {hasRepeatedOrSequentialNumbersRule} from '../hasRepeatedOrSequentialNumbersRules/hasRepeatedOrSequentialNumbersRule';

export function checkPasswordStrengthRule(password: string): 'weak' | 'medium' | 'strong' {

  const regexUpperCase = /[A-Z]/;
  const regexLowerCase = /[a-z]/;
  const regexNumbers = /[0-9]/;
  const regexSpecialChars = /[!@#$%^&*(),.?":{}|<>]/;

  const hasUpperCase = regexUpperCase.test(password);
  const hasLowerCase = regexLowerCase.test(password);
  const hasNumbers = regexNumbers.test(password);
  const hasSpecialChars = regexSpecialChars.test(password);

  const passwordLength = password.length;

  if (passwordLength >= 8 && hasUpperCase && hasLowerCase && hasNumbers && hasSpecialChars) {
    if (!hasRepeatedOrSequentialNumbersRule(password)) {
      return 'strong';
    } else {
      return 'medium';
    }
  } else if (passwordLength >= 6 && ((hasUpperCase && hasLowerCase) || (hasNumbers && hasSpecialChars))) {
    return 'medium';
  } else {
    return 'weak';
  }
}
