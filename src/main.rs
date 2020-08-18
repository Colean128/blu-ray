use serenity::{
    async_trait,
    model::{channel::Message, gateway::Ready},
    prelude::*,
};

struct Handler;
#[async_trait]
impl EventHandler for Handler {
    async fn message(&self, ctx: Context, msg: Message) {
        if msg.content == ("br/ping") {
           if let Err(why) = msg.channel_id.say(&ctx.http, "Ping").await {
              println!("Error sending message: {:?}", why);
              return;
          }
        }
    }
    async fn ready(&self, _: Context, ready: Ready) {
       println!("Connected to Discord as {}#{}.", ready.user.name, ready.user.discriminator)
    }
}

async fn main() {
     let token = env::var("DISCORD_TOKEN")
        .expect("Expected a token in the environment");
     
     let mut client = Client::new(&token)
         .event_handler(Handler)
         .await
         .expect("Client creation error.");
    
     if let Err(why) = client.start().await {
       println!("Client error: {:?}", why);
     }
}
