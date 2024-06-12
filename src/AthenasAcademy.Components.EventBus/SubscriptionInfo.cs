namespace AthenasAcademy.Components.EventBus;

/// <summary>
/// Represents information about an event subscription, including the handler type.
/// </summary>
public class SubscriptionInfo
{
    /// <summary>
    /// Gets the type of the handler associated with the event subscription.
    /// </summary>
    public Type HandlerType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionInfo"/> class with the specified handler type.
    /// </summary>
    /// <param name="handlerType">The type of the handler associated with the event subscription.</param>
    private SubscriptionInfo(Type handlerType)
    {
        HandlerType = handlerType;
    }

    /// <summary>
    /// Creates a new instance of <see cref="SubscriptionInfo"/> with the specified handler type.
    /// </summary>
    /// <param name="handlerType">The type of the handler to be associated with the event subscription.</param>
    /// <returns>A new instance of <see cref="SubscriptionInfo"/> with the specified handler type.</returns>
    public static SubscriptionInfo Typed(Type handlerType)
    {
        return new SubscriptionInfo(handlerType);
    }
}
