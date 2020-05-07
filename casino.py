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

class Casino(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command(pass_context=True)
    async def slot(self, ctx, *, arg: int):
        """Play the slots!"""
        bank = await main.bot_load_bank()
        try:
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
                        await main.bot_save_bank(bank)
                    elif number1 == number2 or number2 == number3:
                        await ctx.send('The machine lights up! You\'ve won!\nThe machine spits out '+str(bet*2)+' Dosh')
                        bank[str(ctx.message.author.id)] = money + bet
                        await main.bot_save_bank(bank)
                    elif number1 == number3:
                        await ctx.send('The machine lights up! You\'ve won!\nThe machine spits out '+str(bet)+' Dosh')
                    else:
                        await ctx.send('The machine plays a sad sound. You\'re in the hole '+str(bet)+' Dosh.')
                        bank[str(ctx.message.author.id)] = money - bet
                        await main.bot_save_bank(bank)
                else:
                    await ctx.send('The machine isn\'t stupid! You clearly don\'t have enough money to afford the bet! Lower your wager!')
        except:
            await ctx.send('Keep your dirty variable away! Integers only!')

def setup(bot):
    bot.add_cog(Casino(bot))
