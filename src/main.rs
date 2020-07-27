use serenity::client::Client;
use serde_json::{Result, Value};
use serenity::model::channel::Message;
use serenity::prelude::{EventHandler, Context};
use serenity::framework::standard::{
    StandardFramework,
    CommandResult,
    macros::{
        command,
        group
    }
};

#[group]
#[commands(ping)]
struct General;

use std::fs;

struct Handler;

impl EventHandler for Handler {}

fn main() {
    let config = fs::read_to_string(config.json)
        .expect("Config not found.");
    let config: Value = serde_json::from_str(config)
    let mut client = Client::new(config["token"].expect("token"), Handler)
        .expect("Error creating client");
    client.with_framework(StandardFramework::new()
        .configure(|c| c.prefix("tbr/"))
        .group(&GENERAL_GROUP));

    if let Err(why) = client.start() {
        println!("An error occurred while running the client: {:?}", why);
    }
}

#[command]
fn ping(ctx: &mut Context, msg: &Message) -> CommandResult {
    msg.reply(ctx, "Pong!")?;

    Ok(())
}