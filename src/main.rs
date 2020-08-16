use serenity::{
    async_trait,
    model::{channel::Message, gateway::Ready},
    prelude::*,
};

let prefix = "br/";
let token = "SNOWFLAKE HERE";

struct Handler;
#[async_trait]
impl EventHandler for Handler {
    async fn message(&self, ctx: Context, msg: Message) {
        if msg.content == ("{}ping", prefix) {
           sendmsg(&self, ctx, msg, "Pong!").await
        }
    }
    async fn ready(&self, _: Context, ready: Ready) {
       println!("Connected to Discord as {}#{}.", ready.user.name, ready.user.discriminator)
    }
}

async fn sendmsg(&self, ctx: Context, msg: Message, n: String)
     if let Err(why) = msg.channel_id.say(&ctx.http, "{}", n).await {
          println!("Error sending message: {:?}", why);
          return;
     }
     return;
}

async fn main() {
     let mut client = Client::new(&token)
         .event_handler(Handler)
         .await
         .expect("Client creation error.");
     if let Err(why) = client.start().await {
       println!("Client error: {:?}", why);
     }
