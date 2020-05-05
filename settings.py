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

class Settings(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True)
        async def sayfilter(self, ctx):
            """[Settings] Toggle the filter for the say command."""
            settings_filter = await main.bot_load_filter()
            if ctx.message.author.id == ctx.message.guild.owner_id or ctx.message.author.id == config.owner:
                if settings_filter.get(str(ctx.message.guild.id)) == None:
                    settings_filter[str(ctx.message.guild.id)] = 1
                    await ctx.send('Filtering disabled!')
                    await main.bot_save_filter(settings_filter)

                elif settings_filter[str(ctx.message.guild.id)] == 0:
                    settings_filter[str(ctx.message.guild.id)] = 1
                    await ctx.send('Filtering disabled!')
                    await main.bot_save_filter(settings_filter)

                elif settings_filter[str(ctx.message.guild.id)] == 1:
                    settings_filter[str(ctx.message.guild.id)] = 0
                    await ctx.send('Filtering enabled!')
                    await main.bot_save_filter(settings_filter)
            else:
                await ctx.send('Only server owners can set this!')

def setup(bot):
    bot.add_cog(Settings(bot))