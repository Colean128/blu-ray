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

class Bank(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True)
    async def bank_register(self, ctx):
        """[Bank] Register a bank account!"""
        bank = await main.bot_load_bank()
        if str(ctx.message.author.id) in bank:
            await ctx.send('Sorry, but you already have an account with the Bank of Sony.')
        else:
            bank[str(ctx.message.author.id)] = 100
            await ctx.send('Thank you for registering with the Bank of Sony!\nYour Account ID number is ' + str(ctx.message.author.id) + ' and your account has been activated.\nYour initial balance is ' + str(bank[str(ctx.message.author.id)]) + ' Dosh')
            await main.bot_save_bank(bank)

    @commands.command(pass_context=True)
    async def balance(self, ctx):
        """[Bank] Check your bank account."""
        bank = await main.bot_load_bank()
        await ctx.send('Bank of Sony ATM\nAccount ID ' + str(ctx.message.author.id) + ' bank balance.\nBalance: ' + str(bank[str(ctx.message.author.id)]) + ' Dosh')

def setup(bot):
    bot.add_cog(Bank(bot))