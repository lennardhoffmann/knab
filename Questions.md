Technical questions:
1. How long did you spend on the coding assignment? What would you add to your solution if you had
more time? If you didn't spend much time on the coding assignment then use this as an opportunity to
explain what you would add.

I spent around 10-12 hours. I wanted to build a really robust solution as I REALLY want to impress Knab.
If I had more time I would build a much better UI, add containerization and even keep track of the search history for a specific currency searched. 
I have already done some of the groundwork for that as I do actually save a history of serch requests for a currency in mongoAtlas


 
2. What was the most useful feature that was added to the latest version of your language of choice?
Please include a snippet of code that shows how you've used it.

System.Text.Json Serialization updates. I used it quite a bit in my solution. This helps to not have to necessarily add a nuget package for json serialization

 public static ExternalCryptoDataProviderResponse DeserializeCryptoData(string jsonResponse)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<ExternalCryptoDataProviderResponse>(jsonResponse, options);
        }


 
3. How would you track down a performance issue in production? Have you ever had to do this?

Logs, logs logs. By identifying the service or functionality which is causing the issue you can further investigate by debugging these piecesof code to determine the root cause of the performance issue
Although on the UI that is not always possible or that easy.


 
4. What was the latest technical book you have read or tech conference you have been to? What did you
learn?

The last tech conference I attended was the AWS summit in Amsterdam in 2023. It peaked my interest in further learning to work with AWS. 
Also gained quite a bit of insight into the architecture of event driven systems



5. What do you think about this technical assessment?

It was a lot of fun. It allowed me to really play around and build a very robust mini application.



6. Please, describe yourself using JSON.
{
    "name": "Lennard Franco Hoffmann",
    "dob": "1989-08-03",
    "nationality": "South African",
    "likes":[
        {
            "name": "Music",
            "description": "I love metal music. I am also teaching myself to play bass guitar"
        },
        {
            "name": "Watching sport",
            "description": "I am an avid sports fan"
        },
        {
            "name": "Gaming",
            "description": "I love gaming, I love to play games. Pc over cnsole. I am also an avid Dnd player and am running an active campaign"
        },
        {
            "name": "Reading",
            "description": "I love reading, I am a huge fan of fantasy and sci-fi novels"
        }
    ],
    "dislikes": [
        {
            "name": "Onions",
            "description": "Just NO"
        },
        {
            "name": "Avocado",
            "description": "See onions"
        },
        {
            "name": "People who are late",
            "description": "I am a stickler for time"
        }       
    ],
    "proudest moment": "Becoming a father. My 2 year old is my life",
    "best attributes":["honesty", "perfectionsim", "loyalty", "hardworking"],
    "worst attributes":[ "overthinker", "honesty", "perfectionsim"]
}


