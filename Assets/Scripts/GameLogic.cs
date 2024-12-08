using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
    TODO:
        - Non-number card functionality
        - currentTurn boolean
        - playing on wild issues
*/

public class GameLogic: MonoBehaviour
{

    public Stack<Card> deck;
    public Stack<Card> pile;
    public List<Card> playerHand;
    public List<Card> enemyHand;
    
    public GameObject cardPrefab;
    public GameObject deckObject;
    public GameObject pileObject;
    public GameObject playerHandObject;
    public GameObject enemyHandObject;

    public bool yourTurn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deck = new Stack<Card>();
        pile = new Stack<Card>();
        playerHand = new List<Card>();
        enemyHand = new List<Card>();

        yourTurn = true;

        InitDeck();
        deck = Shuffle(deck);

        // Deal cards to players hands
        for(int i=0; i<7; ++i){
            Draw(playerHand);
            Draw(enemyHand);
        }
        
        Draw(pile);

        UpdateCards();

        // TODO: Make cards clickable
    }

    // Returns true if given card can be placed on the discard pile
    // Does not check if wild placement is allowed 
    public bool CanPlay(Card c){

        return (
            c.color == Card.Color.BLACK
            || c.color == pile.Peek().color
            || c.number == pile.Peek().number
            || pile.Peek().color == Card.Color.BLACK // debug, remove when wild color pick functional
        );
    }

    public IEnumerator EnemyTurn(){

        yourTurn = false;

        yield return new WaitForSeconds(.5f);

        List<Card> playable = new List<Card>();
        foreach(Card i in enemyHand){
            if(CanPlay(i)) playable.Add(i);
        }
        Card c = new Card();
        if(playable.Count == 0){
            while(true){

                Card i = Draw();
                if(CanPlay(i)){
                    pile.Push(i);
                    break;
                }

                enemyHand.Add(i);
                // Invoke("UpdateCards", .2f);
                UpdateCards();
                yield return new WaitForSeconds(1);
                // TODO: Try to add a pause here at some point
            }
        }
        //TODO: Make a currentTurn flag to prevent player playing cards when enemy drawing
        
        // Check if any cards are not DRAW4, as this card must be played when no others are available
        else if(playable.Count == 1) c = playable[0];

        else if(playable.TrueForAll(i => i.type != Card.Type.DRAW4))
            c = playable[0];
        
        else{
            while(true){
                c = playable[Random.Range(0, playable.Count)];
                if(c.type != Card.Type.DRAW4) break;
                else playable.Remove(c);
            }
        }
           
        pile.Push(c);
        enemyHand.Remove(c);
        UpdateCards();
        yourTurn = true;
    }

    public void PlaceCard(CardPrefab cardPrefab){

        // if(cardPrefab.card.color == Card.Color.BLACK 
        //     || cardPrefab.card.color == pile.Peek().color
        //     || cardPrefab.card.number == pile.Peek().number){ // TODO: Make sure this works with non-number cards
        if(CanPlay(cardPrefab.card) && yourTurn){

                pile.Push(cardPrefab.card);
                playerHand.Remove(cardPrefab.card);
                UpdateCards();

                // Invoke("EnemyTurn", Random.Range(.2f, .8f));
                StartCoroutine(EnemyTurn());
            }
    }

    // Remove all card prefabs in both players hands from the scene. 
    // Could be optimized by specifying which player, but probably not necessary
    void ClearCards(){ foreach(GameObject c in GameObject.FindGameObjectsWithTag("Card")) Destroy(c); }

    // Load correct images for player hand and discard pile
    void UpdateCards(){

        ClearCards();

        pileObject.GetComponent<SpriteRenderer>().sprite = pile.Peek().image;
        
        float x = -playerHand.Count/2;
        for(int i=0; i<playerHand.Count; ++i){
            GameObject newCard = Instantiate(cardPrefab, new Vector3(x + 1*i, -3.75f, 0), Quaternion.identity, playerHandObject.transform);
            newCard.GetComponent<SpriteRenderer>().sprite = playerHand[i].image;
            newCard.GetComponent<CardPrefab>().card = playerHand[i];
            newCard.GetComponent<CardPrefab>().player = true;
        }

        x = -enemyHand.Count/3;
        for(int i=0; i<enemyHand.Count; ++i)
            Instantiate(cardPrefab, new Vector3(x + .5f*i, 1, 0), Quaternion.identity, enemyHandObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Wrapper for deck.Pop() which shuffles discard into deck if no cards are left
    void Draw(List<Card> hand){

        hand.Add(deck.Pop());

        if(deck.Count < 1){
            deck = Shuffle(pile);
            pile.Clear();
        }
    }

    // Wrapper for deck.Pop() which shuffles discard into deck if no cards are left
    void Draw(Stack<Card> hand){

        hand.Push(deck.Pop());

        if(deck.Count < 1){
            deck = Shuffle(pile);
            pile.Clear();
        }
    }
    
    Card Draw(){
        Card c = deck.Pop();

        if(deck.Count < 1){
            deck = Shuffle(pile);
            pile.Clear();
        }

        return c;
    }
    
    void PrintCards(Stack<Card> stack){ foreach(Card c in stack) Debug.Log(c.Stringify()); }

    // Fills deck with starting cards
    void InitDeck(){

        foreach(Card.Color color in System.Enum.GetValues(typeof(Card.Color))){
            if(color == Card.Color.BLACK) continue;

            deck.Push(new Card(color, Card.Type.NUMBER, 0));

            // These cards are 2 per color
            for(int j=0; j<=1; ++j){
                for(int i=1; i<=9; ++i)
                    deck.Push(new Card(color, Card.Type.NUMBER, i));
                
                foreach(Card.Type t in new[]{Card.Type.DRAW2, Card.Type.REVERSE, Card.Type.SKIP})
                    deck.Push(new Card(color, t));
            }
        }

        for(int i=0; i<4; ++i){
            deck.Push(new Card(Card.Color.BLACK, Card.Type.DRAW4));
            deck.Push(new Card(Card.Color.BLACK, Card.Type.WILD));
        }
    }

    // Borrowed from https://stackoverflow.com/questions/273313/randomize-a-listt
    Stack<Card> Shuffle(Stack<Card> stack){

        List<Card> c = stack.ToList();
        int n = c.Count;

        if(n < 2) return stack;
        
        while (n > 1){
            --n;
            int k = Random.Range(0,n+1);
            Card swap = c[k];
            c[k] = c[n];
            c[n] = swap;
        }

        return new Stack<Card>(c);
    }

}
