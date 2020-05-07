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

class Owner(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True,hidden=True)
    async def sfunban(self, ctx, *, arg):
        """Unban someone from Superfilter"""
        if ctx.message.author.id == config.owner:
            settings_superfilterbans = await main.bot_load_sfbans()
            await ctx.send(str(arg)+' has been unbanned from Superfilter.')
            settings_superfilterbans[arg] = 0
            await main.bot_save_sfbans(settings_superfilterbans)

    @commands.command(pass_context=True, hidden=True)
    async def bank_reset(self, ctx):
        """Reset all bank accounts!"""
        if ctx.message.author.id == config.owner:
            bank = await main.bot_load_bank()
            bank.clear()
            await ctx.send('All accounts have been wiped!')
            await main.bot_save_bank(bank)

    @commands.command(pass_context=True, hidden=True)
    async def ibank_reset(self, ctx, arg):
        """Reset a bank account!"""
        if ctx.message.author.id == config.owner:
            bank = await main.bot_load_bank()
            bank.pop(arg, None)
            await ctx.send('Account ID '+str(arg)+' has been wiped.')
            await main.bot_save_bank(bank)

    @commands.command(pass_context=True, hidden=True)
    async def shutdown(self, ctx):
        """Shutdown the bot."""
        if ctx.message.author.id == config.owner:
            await ctx.send('Bot shutting down.')
            print('Logged out.')
            await self.bot.logout()

    @commands.command(pass_context=True, hidden=True)
    async def set_balance(self, ctx, arg1, arg2):
        """Save the bank and shutdown."""
        if ctx.message.author.id == config.owner:
            bank = await main.bot_load_bank()
            bank[str(arg1)] = int(arg2)
            await ctx.send('Set Account ID '+str(arg1)+ '\'s balance to ' + str(arg2) + ' Dosh.')
            await main.bot_save_bank(bank)

    @commands.command(pass_context=True, hidden=True)
    async def eval(self, ctx, argeval):
        """"""
        if ctx.message.author.id == config.owner:
            if inspect.isawaitable(command)
                await ctx.send('```'+await str(eval(str(argeval)))+'```')
            else:
                await ctx.send('```'+str(eval(str(argeval)))+'```')
        else:
            await ctx.send('No eval for you!')

def setup(bot):
    bot.add_cog(Owner(bot))
