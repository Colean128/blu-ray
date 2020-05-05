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

class Info(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self._last_member = None

    @commands.command()
    async def ping(self, ctx):
        """[Info] Play table tennis with the bot."""
        await ctx.send('Pong!')

    @commands.function()
    async def spottoken(self):
        async with aiohttp.ClientSession() as session:
            async with session.post('https://accounts.spotify.com/api/token', headers={'Authorization': 'Basic '+ config.spotifyapikey}, data={"grant_type": "client_credentials"}) as r:
                if r.status == 200:
                    js = await r.json()
                    global spottoke
                    spottoke = (js['access_token'])

    @commands.command()
    async def joindate(self, ctx, *, member: discord.Member):
        """[Info] Tells you the join date of somebody."""
        await ctx.send('{0} joined on {0.joined_at}'.format(member))

    @commands.command()
    async def uptime(self, ctx):
        """[Info] Bot uptime since last reboot"""
        time_diff = round(time.time() - bootsec)
        minute = round(time_diff / 60)
        seconds = time_diff % 60
        if seconds <= 9:
            displaysec = "0"+str(seconds)
            await ctx.send(str(minute)+':'+str(displaysec))
        else:
            await ctx.send(str(minute)+':'+str(seconds))

    @commands.command()
    async def artist(self, ctx, *, arg):
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

    @commands.command()
    async def album(self, ctx, *, arg):
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

    @commands.command()
    async def track(self, ctx, *, arg):
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

    @commands.command()
    async def playlist(self, ctx, *, arg):
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

    @commands.command()
    async def discord(self, ctx):
        """[Info] Join the Blu-Ray Facility discord!"""
        await ctx.send('https://discord.gg/g2SWnrg')

    @commands.command()
    async def invite(self, ctx):
        """[Info] Add the Blu-Ray bot to your server!"""
        await ctx.send('https://discordapp.com/api/oauth2/authorize?client_id=699359348299923517&permissions=0&scope=bot')  

def setup(bot):
    bot.add_cog(Info(bot))