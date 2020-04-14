import discord
import aiohttp
import config
import random
from discord.ext import commands

pogfix = config.prefix

bot = commands.Bot(command_prefix=pogfix)

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

@bot.command(pass_context=True)
async def say(ctx, *, arg):
    """[Fun] Make the bot say stuff."""
    if 'cock and ball torture' in arg or 'cbt' in arg:
        await ctx.send('https://www.youtube.com/watch?v=fR9ClX0egTc All hail the CBT country national anthem.')
    else:
        await ctx.send(arg)

@bot.command(pass_context=True)
async def discord(ctx):
    """[Info] Join the Blu-Ray Facility discord!"""
    await ctx.send('https://discord.gg/g2SWnrg')

@bot.command(pass_context=True)
async def invite(ctx):
    """[Info] Add the Blu-Ray bot to your server!"""
    await ctx.send('https://discordapp.com/api/oauth2/authorize?client_id=699359348299923517&permissions=0&scope=bot')    

bot.run(config.token)
