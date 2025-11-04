namespace BookStoreEcommerce.Messaging.Contracts;

public record InactiveCustomerReengageRequested(
    int CustomerId,
    string? Email,
    DateTime TriggeredAt
);