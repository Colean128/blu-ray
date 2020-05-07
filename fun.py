import discord
import aiohttp
import config
import random
import json
import time
import os
import asyncio
import brfilter
import main
from discord.ext import tasks, commands
brunopowroznik = {0:'https://www.youtube.com/watch?v=6LvlG2dTQKg',1:'https://www.youtube.com/watch?v=ILvd5buCEnU',2:'https://www.youtube.com/watch?v=nEDw_WKeQoc',3:'https://www.youtube.com/watch?v=0YrU9ASVw6w',4:'https://www.youtube.com/watch?v=GxMXWqSauZA',5:'https://www.youtube.com/watch?v=9rtD2omE2N0',6:'https://www.youtube.com/watch?v=-Tqn5NqXskM'}
eightballresponses = {0: 'It is certain.', 1: 'Without a doubt.', 2: 'Yes – definitely.', 3: 'You may rely on it', 4: 'As I see it, yes.', 5: 'Most likely.', 6: 'Outlook good.', 7: 'Yes.', 8: 'Signs point to yes.', 9: 'It is decidedly so.', 10: 'Reply hazy, try again.', 11: 'Ask again later.', 12: 'Better not tell you now.', 13: 'Cannot predict now.', 14: 'Concentrate and ask again.', 15: 'Don\'t count on it.', 16: 'My reply is no.', 17: 'My sources say no.', 18: 'Outlook not so good.', 19: 'Very doubtful.'}

class Fun(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command()
    async def cat(self, ctx):
        """Have a cat picture!"""
        async with aiohttp.ClientSession() as session:
            async with session.get('https://nekos.life/api/v2/img/meow') as r:
                if r.status == 200:
                    js = await r.json()
                    embed = await main.buildEmbed('Here\'s your cat picture!', js['url'])
                    await ctx.send(embed = embed)

    @commands.command(aliases=["8ball"])
    async def eightball(self, ctx, question):
        """Ask me a question!"""
        randomnum = random.randint(0,19)
        await ctx.send(ctx.message.author.nick+' asked the magic 8-ball, **'+str(question)+'**. The magic 8-ball says **'+eightballresponses[randomnum]+'**.')

    @commands.command()
    async def dog(self, ctx):
        """Have a dog picture!"""
        async with aiohttp.ClientSession() as session:
            async with session.get('https://nekos.life/api/v2/img/woof') as r:
                if r.status == 200:
                    js = await r.json()
                    embed = await main.buildEmbed('Here\'s your dog picture!', js['url'])
                    await ctx.send(embed = embed)

    @commands.command()
    async def slap(self, ctx, *, member: discord.Member):
        """Slap somebody around the face!"""
        async with aiohttp.ClientSession() as session:
            async with session.get('https://nekos.life/api/v2/img/slap') as r:
                if r.status == 200:
                    js = await r.json()
                    embed = await main.buildEmbed('{0} slapped {1}!'.format(ctx.message.author.nick, member.nick), js['url'])
                    await ctx.send(embed = embed)

    @commands.command()
    async def hug(self, ctx, *, member: discord.Member):
        """Hug your best friend!"""
        async with aiohttp.ClientSession() as session:
            async with session.get('https://nekos.life/api/v2/img/hug') as r:
                if r.status == 200:
                    js = await r.json()
                    embed = await main.buildEmbed('{0} gave a hug to {1}!'.format(ctx.message.author.nick, member.nick), js['url'])
                    await ctx.send(embed = embed)

    @commands.command()
    async def dice(self, ctx):
        """Roll the dice!"""
        dice = random.randint(1, 6)
        await ctx.send('You rolled the dice and got {}!'.format(dice))

    @commands.command()
    async def feed(self, ctx, *, arg):
        """I am hungry! Feed me!"""
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

    @commands.command(pass_context=True)
    async def why(self, ctx):
        """Why?"""
        async with aiohttp.ClientSession() as session:
            async with session.get('https://nekos.life/api/v2/why') as r:
                if r.status == 200:
                    js = await r.json()
                    await ctx.send(js['why'])

    @commands.command()
    async def say(self, ctx, *, arg):
        """Make the bot say stuff."""
        settings_superfilterbans = await main.bot_load_sfbans()
        settings_filter = await main.bot_load_filter()
        if any(s in arg.lower() for s in brfilter.badwords):
            if settings_filter.get(str(ctx.message.guild.id)) == None:
                # print(str(ctx.message.author.id) +' Tried to send ' + str(arg) +' to server ID ' + str(ctx.message.guild.id) + ' with filtering on')
                await ctx.send('Your message contains filtered words!')

            elif settings_filter[str(ctx.message.guild.id)] == 0:
                # print(str(ctx.message.author.id) +' Tried to send ' + str(arg) +' to server ID ' + str(ctx.message.guild.id) + ' with filtering on')
                await ctx.send('Your message contains filtered words!')

            else:
                # print(str(ctx.message.author.id) +' Sent ' + str(arg) +' to server ID ' + str(ctx.message.guild.id) + ' with filtering off')
                await ctx.send(arg)
        elif any(s in arg.lower() for s in brfilter.superbadwords):
            if settings_superfilterbans.get(str(ctx.message.author.id)) == None:
                settings_superfilterbans[str(ctx.message.author.id)] = 0
                msg = ctx.message
                await msg.delete()
                await ctx.send('```Superfilter Alert\nYour message contained super filtered words!\nThe next time you use those, I\'ll have to ban you from this command!```')
                await main.bot_save_sfbans(settings_superfilterbans)
            elif settings_superfilterbans[str(ctx.message.author.id)] == 0:
                settings_superfilterbans[str(ctx.message.author.id)] = 1
                msg = ctx.message
                await msg.delete()
                await ctx.send('```Superfilter Alert\nYour message contained super filtered words!\nYou\'ve been banned from the say command.\nJoin our support server to appeal the ban.```')
                await ctx.send('https://discord.gg/g2SWnrg')
                await main.bot_save_sfbans(settings_superfilterbans)
        elif settings_superfilterbans.get(str(ctx.message.author.id)) != None:
            if settings_superfilterbans[str(ctx.message.author.id)] == 1:
                msg = ctx.message
                await msg.delete()
                await ctx.send('```Superfilter Alert\nYou\'ve been banned from the say command.\nJoin our support server to appeal the ban.```')
                await ctx.send('https://discord.gg/g2SWnrg')
            else:
                await ctx.send(arg)
        else:
            await ctx.send(arg)

    @commands.command()
    async def bruno(self, ctx):
        """A Bruno Powroznik special."""
        settings_filter = await main.bot_load_filter()
        if settings_filter.get(str(ctx.message.guild.id)) == None:
            await ctx.send('This server has filtering enabled.')

        elif settings_filter[str(ctx.message.guild.id)] == 0:
            await ctx.send('This server has filtering enabled.')

        else:
            x = random.randint(0,6)
            await ctx.send(brunopowroznik[x])

def setup(bot):
    bot.add_cog(Fun(bot))
