---
name: Bug Report
about: Create a bug report
title: "[BUG]"
labels: bug, feature
assignees: ''

---

### Summary
[Insert a concise, descriptive title of the bug. What is the issue?]

### Details
- **App version:** [e.g., 1.0.0]
- **Reproducibility:** [e.g., Always (100%), Intermittent, Only once]
- **Severity:**
    - [ ] Blockover
    - [ ] Major
    - [ ] Minor
    - [ ] Trivial
- **Priority:**
    - [ ] High
    - [ ] Medium
    - [ ] Low

### Steps to Reproduce
1. [First Step - e.g., Open the app and log in]
2. [Second Step - e.g., Add chat]
3. [Third Step - e.g., write !help]
4. **Actual Result:** [What actually happens? e.g., The app crashes with a white screen]
5. **Expected Result:** [What should happen? e.g., Bot should respond]

---

### Technical Context
- **Error Logs/Trace:** [Paste error codes, stack traces, or links to monitoring tools like Sentry/CloudWatch]
- **Data Handling:** [Mention specific IDs, payload examples, or timestamp issues involved]
- **Integration:** [Which API or third-party service is failing?]

### Impact
- **Workflow:** [How does this stop the user? e.g., Prevents completion of checkout]
- **Reliability:** [Does this cause data corruption or sync issues?]
- **User Experience:** [e.g., Highly frustrating, leads to user churn]

### Acceptance Criteria (Fix Verification)
- [ ] **Functional:** The bug is no longer reproducible using the steps above.
- [ ] **Stability:** The fix handles edge cases (e.g., empty fields, slow network) without crashing.
- [ ] **Regression:** Existing features in the same module continue to function correctly.
