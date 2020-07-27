use std::fs::{self, File};
use std::io::BufReader;

use serenity::client::Client;
use serenity::framework::standard::{
	CommandResult,
	macros::{
		command,
		group
	},
	StandardFramework
};
use serenity::model::channel::Message;
use serenity::prelude::{Context, EventHandler};

use serde::{Result, Value};

#[group]
#[commands(ping)]
struct General;

struct Handler;

struct Config {
	token: String
}

impl EventHandler for Handler {}

fn main() {
	struct ConfigIni { token: String }
	let path = matches.value_of("config-ini").unwrap();
	let canonicalized = std::fs::canonicalize(path).context("couldn't resolve ini path")?;
	let file = File::open(canonicalized).context("couldn't open Config .ini")?;
	let deserialized: DigitalOceanIni = serde_ini::from_read(file).context("couldn't parse .ini")?;

	let mut client = Client::new(deserialized.token, Handler)
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
