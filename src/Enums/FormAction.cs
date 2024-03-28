namespace MudBlazor.StaticInput;

public enum FormAction
{
    /// <summary>
    ///     Sets the button type to submit
    /// </summary>
    Submit,
    /// <summary>
    ///     Sets the button type to reset
    /// </summary>
    Reset,
    /// <summary>
    ///     Sets the button type to submit and wraps the button within a from element with method="post"
    /// </summary>
    Post
}