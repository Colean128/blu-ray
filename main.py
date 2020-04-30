import discord
import aiohttp
import config
import random
import json
import time
import os
import asyncio
import brfilter
from discord.ext import tasks, commands

pogfix = config.prefix
bank = {}

bot = commands.Bot(command_prefix=pogfix)

@bot.command(pass_context=True)
async def bank_register(ctx):
    """[Bank] Register a bank account!"""
    if str(ctx.message.author.id) in bank:
        await ctx.send('Sorry, but you already have an account with the Bank of Sony.')
    else:
        bank[str(ctx.message.author.id)] = 100
        await ctx.send('Thank you for registering with the Bank of Sony!\nYour Account ID number is ' + str(ctx.message.author.id) + ' and your account has been activated.\nYour initial balance is ' + str(bank[str(ctx.message.author.id)]) + ' Dosh')

@bot.command(pass_context=True, hidden=True)
async def bank_reset(ctx):
    """[Owner] Reset all bank accounts!"""
    if ctx.message.author.id == config.owner:
        bank.clear()
        await ctx.send('All accounts have been wiped!')

@bot.command(pass_context=True, hidden=True)
async def ibank_reset(ctx, arg):
    """[Owner] Reset a bank account!"""
    if ctx.message.author.id == config.owner:
        bank.pop(arg, None)
        await ctx.send('Account ID '+str(arg)+' has been wiped.')

@bot.command(pass_context=True, hidden=True)
async def dumpbank(ctx):
    """[Debug] Dumps bank dictionary to a file."""
    if ctx.message.author.id == config.owner:
        # with open('dumpbank_buffer.json', 'w') as f:
        #   json.dump(bankb, f)
        #   await ctx.send('Dumped bank bank to file')
        #   f.close()
        with open('dumpbank_dictionary.json', 'w') as f:
            json.dump(bank, f)
            await ctx.send('Dumped bank dictionary to file')
            f.close()

@bot.command(pass_context=True, hidden=True)
async def saybank(ctx):
    """[Debug] Replies with bank dictionary."""
    if ctx.message.author.id == config.owner:
        await ctx.send(str(bank))

@bot.command(pass_context=True, hidden=True)
async def savebank(ctx):
    """[Bank] Save the bank dictionary to a file."""
    if ctx.message.author.id == config.owner:
        msg = await ctx.send('Saving bank balances.')
        with open('save_bank.json', 'w') as f:
            json.dump(bank, f)
            await msg.edit(content='Saved bank balances to file')
            f.close()

@bot.command(pass_context=True, hidden=True)
async def shutdown(ctx):
    """[Owner] Save the bank and shutdown."""
    if ctx.message.author.id == config.owner:
        msg = await ctx.send('Saving bank balances.')
        with open('settings_filter.json', 'w') as f:
            json.dump(settings_filter, f)
            f.close()
        with open('save_bank.json', 'w') as f:
            json.dump(bank, f)
            await msg.edit(content='Saved bank balances to file.\nGood night!\n*Cave Story Theme starts to loop.*')
            f.close()
        print('Logged out')
        await bot.logout()

@bot.command(pass_context=True, hidden=True)
async def set_balance(ctx, arg1, arg2):
    """[Owner] Save the bank and shutdown."""
    if ctx.message.author.id == config.owner:
        bank[str(arg1)] = int(arg2)
        await ctx.send('Set Account ID '+str(arg1)+ '\'s balance to ' + str(arg2) + ' Dosh.')

@bot.command(pass_context=True)
async def balance(ctx):
    """[Bank] Check your bank account."""
    await ctx.send('Bank of Sony ATM\nAccount ID ' + str(ctx.message.author.id) + ' bank balance.\nBalance: ' + str(bank[str(ctx.message.author.id)]) + ' Dosh')

@bot.command(pass_context=True)
async def slot(ctx, *, arg):
    """[Casino] Play the slots!"""
    if int(arg) <= 9:
        await ctx.send('You have to bet at least 10 Dosh!')
    else:
        if bank[str(ctx.message.author.id)] >= int(arg):
            await ctx.send('The machine takes your Dosh. You pull the lever.')
            bet = int(arg)
            money = bank[str(ctx.message.author.id)]
            number1 = random.randint(0,9)
            number2 = random.randint(0,9)
            number3 = random.randint(0,9)
            await ctx.send('The display says '+str(number1)+' '+str(number2)+' '+str(number3)+'.')
            if number1 == number2 and number2 == number3:
                await ctx.send('The machine lights up and flashes! You\'ve won the jackpot!\nThe machine spits out '+str(bet*4)+' Dosh')
                bank[str(ctx.message.author.id)] = money + bet*3
            elif number1 == number2 or number2 == number3:
                await ctx.send('The machine lights up! You\'ve won!\nThe machine spits out '+str(bet*2)+' Dosh')
                bank[str(ctx.message.author.id)] = money + bet
            elif number1 == number3:
                await ctx.send('The machine lights up! You\'ve won!\nThe machine spits out '+str(bet)+' Dosh')
            else:
                await ctx.send('The machine plays a sad sound. You\'re in the hole '+str(bet)+' Dosh.')
                bank[str(ctx.message.author.id)] = money - bet
        else:
            await ctx.send('The machine isn\'t stupid! You clearly don\'t have enough money to afford the bet! Lower your wager!')

@bot.command(pass_context=True)
async def ping(ctx):
    """[Info] Play table tennis with the bot."""
    await ctx.send('Pong!')

@bot.command(pass_context=True)
async def joindate(ctx, *, member: discord.Member):
    """[Info] Tells you the join date of somebody."""
    await ctx.send('{0} joined on {0.joined_at}'.format(member))

@bot.command(pass_context=True)
async def slap(ctx, *, member: discord.Member):
    """[Fun] Slap somebody around the face!"""
    await ctx.send('{0} slapped {1}!'.format(ctx.message.author.mention, member.mention))
    async with aiohttp.ClientSession() as session:
        async with session.get('https://nekos.life/api/v2/img/slap') as r:
            if r.status == 200:
                js = await r.json()
                await ctx.send(js['url'])

@bot.command(pass_context=True)
async def hug(ctx, *, member: discord.Member):
    """[Fun] Hug your best friend!"""
    await ctx.send('{0} gave a hug to {1}!'.format(ctx.message.author.mention, member.mention))
    async with aiohttp.ClientSession() as session:
        async with session.get('https://nekos.life/api/v2/img/cuddle') as r:
            if r.status == 200:
                js = await r.json()
                await ctx.send(js['url'])

@bot.command(pass_context=True)
async def dice(ctx):
    """[Fun] Roll the dice!"""
    dice = random.randint(1, 6)
    await ctx.send('You rolled the dice and got {}!'.format(dice))

@bot.command(pass_context=True)
async def feed(ctx, *, arg):
    """[Fun] I am hungry! Feed me!"""
    if '🍆' in arg or 'eggplant' in arg:
        await ctx.send('I don\'t want to bite down on it, I\'d rather suck the juice out of it!')
        await ctx.send('Yum, yum eggplant juice!')
    elif '🍕' in arg or 'pizza' in arg:
        await ctx.send('_lipsmack_ Mmm. I love how creamy the sauce is!')
    elif '🇬🇧' in arg or 'uk flag' in arg:
        await ctx.send('I refuse to eat the UK flag! Unepic!')
    elif '🇩🇪' in arg or 'german flag' in arg:
        await ctx.send('Ich werde die deutsche Flagge nicht essen! Unepisch!')
    elif '🇧🇪' in arg or 'belgium flag' in arg:
        await ctx.send('Hmm. Germany on it\'s side? I\'ll eat it. _rip_ _crunch_ Tastes meh.')
    elif '🏳️‍🌈' in arg or 'rainbow flag' in arg:
        await ctx.send('I- I can\'t eat that flag! I would anger my creator!')
    elif '🍋' in arg or 'lemon' in arg:
        await ctx.send('✝️ KEEP THAT SATANIC YELLOW CITRUS RUTACEAE SAPINDALE ORGANISM AWAY FROM ME!!!! ✝️')
    elif '🍌' in arg or 'banana' in arg:
        await ctx.send('Nice and ripe! _munch munch_')
    elif '🥓' in arg or 'bacon' in arg:
        await ctx.send('Bacon, eggs and bangers and mash!. Now that\'s a traditional english breakfast!')
        await ctx.send('Maybe add some yorkshire pudding.')
    elif '🍑' in arg or 'peach' in arg or 'butt' in arg:
        await ctx.send('_lick_ Can I lick it more?')
    elif '🥵' in arg or 'hot face' in arg or 'moan face' in arg:
        await ctx.send('Feed me your eggplant.')
    elif '🦇' in arg or 'bat' in arg:
        await ctx.send('🤢 Corona! Corona!')
    elif '😳' in arg or 'flushed' in arg:
        await ctx.send('Huh?')
    elif '🥩' in arg or 'meat' in arg:
        await ctx.send('_gets meat hammer_ Time to beat that meat.')
    elif '🐈' in arg or 'cat' in arg or '🐱' in arg:
        await ctx.send('No! You take the pussy!')
    elif '🍄' in arg or 'mushroom' in arg:
        await ctx.send('It\'s a-me, a Drug addict!')
    else:
        await ctx.send('_munch_ I like the taste!')

@bot.command(pass_context=True)
async def why(ctx):
    """[Fun] Why?"""
    async with aiohttp.ClientSession() as session:
        async with session.get('https://nekos.life/api/v2/why') as r:
            if r.status == 200:
                js = await r.json()
                await ctx.send(js['why'])

@bot.command(pass_context=True, hidden=True)
async def r34(ctx, *, arg):
    """[NSFW] Search Rule34.
    Command restricted to NSFW channels."""
    if ctx.message.channel.is_nsfw():
        async with aiohttp.ClientSession() as session:
            async with session.get('https://r34-json.herokuapp.com/posts?tags='+arg) as r:
                if r.status == 200:
                    js = await r.json()
                    await ctx.send(js)
                else:
                    await ctx.send('Search failed.')
    else:
        await ctx.send("This command is restricted to NSFW channels.")

@bot.command(pass_context=True)
async def cat(ctx):
    """[Fun] Have a cat picture!"""
    async with aiohttp.ClientSession() as session:
        async with session.get('https://nekos.life/api/v2/img/meow') as r:
            if r.status == 200:
                js = await r.json()
                await ctx.send(js['url'])

@bot.command(pass_context=True)
async def dog(ctx):
    """[Fun] Have a cat picture!"""
    async with aiohttp.ClientSession() as session:
        async with session.get('https://nekos.life/api/v2/img/woof') as r:
            if r.status == 200:
                js = await r.json()
                await ctx.send(js['url'])

async def spottoken():
    async with aiohttp.ClientSession() as session:
            async with session.post('https://accounts.spotify.com/api/token', headers={'Authorization': 'Basic '+ config.spotifyapikey}, data={"grant_type": "client_credentials"}) as r:
                if r.status == 200:
                    js = await r.json()
                    global spottoke
                    spottoke = (js['access_token'])

@bot.command(pass_context=True)
async def uptime(ctx):
    """[Info] Bot uptime since last reboot"""
    time_diff = round(time.time() - bootsec)
    minute = round(time_diff / 60)
    seconds = time_diff % 60
    if seconds <= 9:
        displaysec = "0"+str(seconds)
        await ctx.send(str(minute)+':'+str(displaysec))
    else:
        await ctx.send(str(minute)+':'+str(seconds))

@bot.command(pass_context=True)
async def artist(ctx, *, arg):
    """[Info] Search for artists on Spotify."""
    await spottoken()
    async with aiohttp.ClientSession() as session:
        async with session.get('https://api.spotify.com/v1/search?q='+arg+'&type=artist&limit=1', headers={'Authorization': 'Bearer '+ spottoke}) as r1:
            if r1.status == 200:
                # Note to self: don't fuck with this code, you'll probably spend two days fixing it.
                js = await r1.json()
                jsparse = js['artists']['items'][0]['external_urls']['spotify']
                await ctx.send('Is this the artist you were looking for? '+jsparse)
            else:
                print(r1.status)

@bot.command(pass_context=True)
async def album(ctx, *, arg):
    """[Info] Search for albums on Spotify."""
    await spottoken()
    async with aiohttp.ClientSession() as session:
        async with session.get('https://api.spotify.com/v1/search?q='+arg+'&type=album&limit=1', headers={'Authorization': 'Bearer '+ spottoke}) as r1:
            if r1.status == 200:
                js = await r1.json()
                jsparse = js['albums']['items'][0]['external_urls']['spotify']
                await ctx.send('Is this the album you were looking for? '+jsparse)
            else:
                print(r1.status)

@bot.command(pass_context=True)
async def track(ctx, *, arg):
    """[Info] Search for tracks on Spotify."""
    await spottoken()
    async with aiohttp.ClientSession() as session:
        async with session.get('https://api.spotify.com/v1/search?q='+arg+'&type=track&limit=1', headers={'Authorization': 'Bearer '+ spottoke}) as r1:
            if r1.status == 200:
                js = await r1.json()
                jsparse = js['tracks']['items'][0]['external_urls']['spotify']
                await ctx.send('Is this the track you were looking for? '+jsparse)
            else:
                print(r1.status)

@bot.command(pass_context=True)
async def playlist(ctx, *, arg):
    """[Info] Search for playlists on Spotify."""
    await spottoken()
    async with aiohttp.ClientSession() as session:
        async with session.get('https://api.spotify.com/v1/search?q='+arg+'&type=playlist&limit=1', headers={'Authorization': 'Bearer '+ spottoke}) as r1:
            if r1.status == 200:
                js = await r1.json()
                jsparse = js['playlists']['items'][0]['external_urls']['spotify']
                await ctx.send('Is this the playlist you were looking for? '+jsparse)
            else:
                print(r1.status)

@bot.command(pass_context=True)
async def say(ctx, *, arg):
    """[Fun] Make the bot say stuff."""
    if any(s in arg for s in brfilter.badwords):
        if settings_filter.get(str(ctx.message.guild.id)) == None:
            # print(str(ctx.message.author.id) +' Tried to send ' + str(arg) +' to server ID ' + str(ctx.message.guild.id) + ' with filtering on')
            await ctx.send('Your message contains filtered words!')

        elif settings_filter[str(ctx.message.guild.id)] == 0:
            # print(str(ctx.message.author.id) +' Tried to send ' + str(arg) +' to server ID ' + str(ctx.message.guild.id) + ' with filtering on')
            await ctx.send('Your message contains filtered words!')

        else:
            # print(str(ctx.message.author.id) +' Sent ' + str(arg) +' to server ID ' + str(ctx.message.guild.id) + ' with filtering off')
            await ctx.send(arg)
    else:
        await ctx.send(arg)

@bot.command(pass_context=True)
async def sayfilter(ctx):
    """[Settings] Toggle the filter for the say command."""
    if ctx.message.author.id == ctx.message.guild.owner_id:
        if settings_filter.get(str(ctx.message.guild.id)) == None:
            settings_filter[str(ctx.message.guild.id)] = 1
            await ctx.send('Filtering disabled!')

        elif settings_filter[str(ctx.message.guild.id)] == 0:
            settings_filter[str(ctx.message.guild.id)] = 1
            await ctx.send('Filtering disabled!')

        elif settings_filter[str(ctx.message.guild.id)] == 1:
            settings_filter[str(ctx.message.guild.id)] = 0
            await ctx.send('Filtering enabled!')

    else:
        await ctx.send('Only server owners can set this!')

@bot.command(pass_context=True, hidden=True)
async def sayset(ctx):
    """[Debug] Replies with settings."""
    if ctx.message.author.id == config.owner:
        await ctx.send(str(settings_filter))

@bot.command(pass_context=True, hidden=True)
async def saveset(ctx):
    """[Settings] Save settings to a file."""
    if ctx.message.author.id == config.owner:
        msg = await ctx.send('Saving settings.')
        with open('settings_filter.json', 'w') as f:
            json.dump(settings_filter, f)
            await msg.edit(content='Saved settings to file')
            f.close()

@bot.command(pass_context=True)
async def discord(ctx):
    """[Info] Join the Blu-Ray Facility discord!"""
    await ctx.send('https://discord.gg/g2SWnrg')

@bot.command(pass_context=True)
async def invite(ctx):
    """[Info] Add the Blu-Ray bot to your server!"""
    await ctx.send('https://discordapp.com/api/oauth2/authorize?client_id=699359348299923517&permissions=0&scope=bot')    

print('Loading bank.')
if os.path.exists('save_bank.json') == True:
    f = open('save_bank.json')
    bank = json.load(f)
    # print(bank)
    f.close()
else:
    bank = {}

print('Loading settings.')
if os.path.exists('settings_filter.json') == True:
    f = open('settings_filter.json')
    settings_filter = json.load(f)
    # print(settings_filter)
    f.close()
else:
    settings_filter = {}


print('Bot running.')
bootsec = time.time()
bot.run(config.token)