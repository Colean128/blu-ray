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
headers = {'User-Agent': 'Blu-Ray 1.1.2 (Discord bot by Zayne64 on GitHub)'}
class NSFW(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True, aliases=['rule34'])
    async def r34(self, ctx, *, arg):
        """Search Rule34.
        Command restricted to NSFW channels."""
        if ctx.message.channel.is_nsfw():
            async with aiohttp.ClientSession(headers=headers) as session:
                async with session.get('https://r34-json-api.herokuapp.com/posts?tags='+str(arg)) as r:
                    if r.status == 200:
                        js = await r.json()
                        rand = random.randint(1,100)
                        if '.webm' in js[rand]['file_url']:
                            await ctx.send('Rule34 search results for **'+str(arg)+'**.')
                            await ctx.send('Video: '+js[rand]['file_url'])
                        else:
                            embed = await main.buildEmbed('Rule34 search results for **'+str(arg)+'**.', js[rand]['file_url'])
                            await ctx.send(embed = embed)
                    else:
                        await ctx.send('Search failed.')
        else:
            await ctx.send("This command is restricted to NSFW channels.")

    @commands.command(pass_context=True)
    async def fuck(self, ctx, *, member: discord.User):
        """Fuck your friend.
        Command restricted to NSFW channels."""
        if ctx.message.channel.is_nsfw():
            async with aiohttp.ClientSession() as session:
                async with session.get('https://nekos.life/api/v2/img/classic') as r:
                    if r.status == 200:
                        js = await r.json()
                        embed = await main.buildEmbed('{0} fucked {1}!'.format(ctx.message.author.name, member.name), js['url'])
                        await ctx.send(embed = embed)

def setup(bot):
    bot.add_cog(NSFW(bot))
