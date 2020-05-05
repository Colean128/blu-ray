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

class NSFW(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True)
    async def r34(self, ctx, arg):
        """Search Rule34.
        Command restricted to NSFW channels."""
        if ctx.message.channel.is_nsfw():
            async with aiohttp.ClientSession() as session:
                async with session.get('https://r34-json-api.herokuapp.com/posts?tags='+str(arg)) as r:
                    if r.status == 200:
                        js = await r.json()
                        rand = random.randint(1,20)
                        jsparse = js[rand]['file_url']
                        await ctx.send(jsparse)
                    else:
                        await ctx.send('Search failed.')
        else:
            await ctx.send("This command is restricted to NSFW channels.")

def setup(bot):
    bot.add_cog(NSFW(bot))