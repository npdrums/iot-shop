# IOT Web Shop

To run this app you need ASP.NET Core 3.1, Angular 10, Redis and Stripe integration. 
You would also need to generate self-signed certificates to run over SSL.

### Prerequisites

1. Register your account on [Stripe](https://stripe.com) and create new business
2. Install [Stripe CLI](https://stripe.com/docs/stripe-cli) and connect it to your Stripe account
3. Configure [Stripe Secrets](https://stripe.com/docs/payments/payment-intents) and [Stripe WebHook](https://stripe.com/docs/webhooks) in appsettings.json
3. Install [Redis](https://redis.io/)

### How to run?

1. You need at least 3 terminal instances
2. Run `stripe login` and follow prompt to connect to your stripe account in the first terminal
3. Run `stripe listen` in the same window
4. Run `redis-server` in the second terminal
5. Run `redis-cli` in the third terminal to take control over Redis server
6. Run ASP.NET Core and Angular apps, respectivly