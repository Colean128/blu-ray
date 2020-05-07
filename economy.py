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

class Economy(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True)
    @commands.cooldown(1, 7200, <BucketType.user: 0>)
    async def work(self, ctx):
        """Work for your money."""
        bank = await main.bot_load_bank()
        money = random.randint(0,40)
        await ctx.send('You did odd jobs around the neighbourhood and made '+str(money)+' Dosh.')
        bank[str(ctx.message.author.id)] = bank[str(ctx.message.author.id)] + money
        await main.bot_save_bank(bank)

def setup(bot):
    bot.add_cog(Economy(bot))
