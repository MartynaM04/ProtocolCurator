# Contributing to Protocol Curator

Thank you for your interest in contributing to Protocol Curator! 

## How to Contribute

### Reporting Bugs

If you find a bug, please create an issue with:
- **Clear title** describing the problem
- **Steps to reproduce** the bug
- **Expected behavior** vs actual behavior
- **Unity version** and platform (Windows/Mac/Linux)
- **Screenshots or video** if applicable

### Suggesting Features

Feature requests are welcome! Please:
- Check existing issues to avoid duplicates
- Describe the feature clearly
- Explain why it would be valuable
- Consider implementation complexity

### Code Contributions

1. **Fork the repository**
2. **Create a feature branch** (`git checkout -b feature/YourFeature`)
3. **Follow code style** (see CODE_README.md)
4. **Test thoroughly** in Unity Editor
5. **Commit with clear messages** (`git commit -m 'Add feature: description'`)
6. **Push to your fork** (`git push origin feature/YourFeature`)
7. **Create a Pull Request**

### Code Style Guidelines

- **C# Naming:** PascalCase for classes/methods, camelCase for variables
- **Comments:** Use XML documentation (`/// <summary>`) for public methods
- **Unity Conventions:** Use `[Header]`, `[SerializeField]`, and organize Inspector fields
- **Input System:** Use New Input System, not legacy Input
- **Version Control:** Unity 6.0+ compatible code only

### Pull Request Checklist

- [ ] Code follows the project's style guidelines
- [ ] Changes tested in Unity Editor
- [ ] No console errors or warnings
- [ ] Documentation updated (if needed)
- [ ] Commit messages are clear and descriptive
- [ ] No merge conflicts

## Testing

Before submitting:
1. Test in Unity Editor (Play Mode)
2. Verify all artifact interactions work
3. Check save/load functionality
4. Test pause menu
5. Ensure no console errors

## Documentation

If your contribution affects:
- **Core scripts:** Update CODE_README.md
- **Gameplay:** Update README.md
- **Setup:** Update INSTALLATION_CHECKLIST.md

## AI-Assisted Development

This project uses AI assistance (Claude by Anthropic). If you use AI tools:
- Disclose AI usage in pull requests
- Review and understand all AI-generated code
- Test AI code thoroughly
- Maintain code quality standards

## Priority Areas

Help is especially welcome in:
- **Gamepad support** - Currently keyboard/mouse only
- **Save system** - Moving from PlayerPrefs to JSON
- **Localization** - Multi-language support
- **Mobile support** - Touch controls and optimization
- **Bug fixes** - See open issues

## Questions?

- Check CODE_README.md for technical details
- Review existing issues and discussions
- Open an issue for questions about contributing

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for making Protocol Curator better!**
