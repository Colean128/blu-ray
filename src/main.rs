mod commands;

use std::{
	collections::HashSet,
	env,
	sync::Arc,
};
use serenity::{
	client::bridge::gateway::ShardManager,
	framework::{
		StandardFramework,
		standard::macros::group,
	},
	model::{event::ResumedEvent, gateway::Ready},
	prelude::*,
};
use log::{error, info};

use commands::{
	info::*
};
struct ShardManagerContainer;

impl TypeMapKey for ShardManagerContainer {
	type Value = Arc<Mutex<ShardManager>>;
}

struct Handler;

impl EventHandler for Handler {
	fn ready(&self, _: Context, ready: Ready) {
		info!("Connected as {}#{}", ready.user.name, ready.user.discriminator);
	}

	fn resume(&self, _: Context, _: ResumedEvent) {
		info!("Resumed");
	}
}

#[group]
#[commands(ping)]
struct General;

fn main() {

	kankyo::load().expect(".env file not found.");
	env_logger::init();

	let token = env::var("TOKEN")
		.expect("Discord Token not found in enviroment.");

	let mut client = Client::new(&token, Handler).expect("Client creation error.");

	{
		let mut data = client.data.write();
		data.insert::<ShardManagerContainer>(Arc::clone(&client.shard_manager));
	}

	let owners = match client.cache_and_http.http.get_current_application_info() {
		Ok(info) => {
			let mut set = HashSet::new();
			set.insert(info.owner.id);

			set
		},
		Err(why) => panic!("Couldn't get application info: {:?}", why),
	};

	client.with_framework(StandardFramework::new()
		.configure(|c| c
			.owners(owners)
			.prefix("tbr/"))
		.group(&GENERAL_GROUP));

	if let Err(why) = client.start() {
		error!("Client error: {:?}", why);
	}
}
