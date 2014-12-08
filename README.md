---
...

#A Deeper Exploration of HPC GPU Computing:

#CUDA and Beyond

  

  

  

  

  

  

  

  

  

  

  

  

  

  

  

Antony Drew – LUC – COMP 490

Fall 2014

[adrew@luc.edu](mailto:adrew@luc.edu)



##Table of Contents 
=================

[Introduction – What are we trying to accomplish?
3](#__RefHeading__4498_1905951770)

[What is HPC Hybrid GPU Computing? Matrices, vectors and more…
3](#__RefHeading__4500_1905951770)

[Distributed System Summary of GPU Server:
6](#__RefHeading__4502_1905951770)

[Why do we need GPU Computing? Hitting the wall…
6](#__RefHeading__4504_1905951770)

[Testing Approach, Setup & Hypothesis 8](#__RefHeading__4508_1905951770)

[[TODO] 8](#__RefHeading__2366_312021579)

[Description of GPU Server Operations & Architecture
9](#__RefHeading__4514_1905951770)

[A CPU Task versus a GPU Task: Pizza Delivery
9](#__RefHeading__4516_1905951770)

[What does a GPU chip look like? Cores, Caches & SM’s
9](#__RefHeading__4518_1905951770)

[Hybrid CPU-GPU Operations: Processing Flow
10](#__RefHeading__4520_1905951770)

[A Simple CUDA C Function 11](#__RefHeading__4524_1905951770)

[A Simple CUDA C Program 11](#__RefHeading__4526_1905951770)

[A More Complex CUDA C Program: Model Optimization in Finance
13](#__RefHeading__4530_1905951770)

[A Primer in Finance & Building Models – The “Martians” Have Landed
13](#__RefHeading__4532_1905951770)

[Using Wrappers and Other “Non-native” Packages for Modelling: What are
the tradeoffs of using these alternatives?
23](#__RefHeading__4542_1905951770)

[The MATLAB Stats package 23](#__RefHeading__4544_1905951770)

[[TODO] 24](#__RefHeading__4581_1905951770)

[JCUDA 25](#__RefHeading__4546_1905951770)

[[TODO] 26](#__RefHeading__4548_1905951770)

[Extreme GPU Computing: Master Level Complexity
26](#__RefHeading__4550_1905951770)

[Using Multiple GPU’s (Server/Cluster) & Streams for Further Speed Gains
in Model Program – Multi-GPU’s & Multi-Streams (Plus Advanced Math
Functions) 26](#__RefHeading__4552_1905951770)

[Amdhal’s Law 27](#__RefHeading__4554_1905951770)

[Conclusion and Wrap-Up: Did we prove out speed gains?
28](#__RefHeading__4556_1905951770)

[[TODO] 28](#__RefHeading__4558_1905951770)

[Bibliography 29](#__RefHeading__4562_1905951770)



##Introduction – What are we trying to accomplish? 
================================================

The aim of this project is to explore the potential benefits of GPU
computing in its various forms. By laying the groundwork through working
examples and exposition, various speed benefits will be demonstrated and
also shown to be surmountable by the average practitioner. This subject
should not be daunting or deemed necessarily out-of-reach by the typical
person. If certain rules can be kept straight, code samples of
increasing complexity are attainable. Hopefully, this demonstration is
persuasive and can induce others into scientific computing, especially,
younger generations.

##What is HPC Hybrid GPU Computing? Matrices, vectors and more… 
=============================================================

“HPC” stands for high-performance computing. “Hybrid” means that both
the CPU and GPU are being targeted for various calculations in order to
share the workload. Previously, pools of “workers” or CPU’s (e.g.,
AMD/INTEL chips) were coordinated together to perform vast numerical
computations in a distributed fashion. These are often referred to as
“clusters” or “farms”. However, in the late 1990’s, academics on the
West Coast along with companies like Nvidia began to explore libraries
to take advantage of the GPU – ‘GPU’ here means nothing more than the
chip on a video card typically used to render graphics on a computer
screen – when several of these are encased in a server, then we have a
cluster. As GPU technology became more advanced by
1999[^1^](#sdfootnote1sym), practitioners in the science and finance
fields began to incorporate GPU clusters into their operations. Remember
that graphics rendering typically involves mathematical vector or matrix
operations to draw lines and curves. This word ‘matrix’ (e.g., a bunch
of columns or vectors) is important because many computations in various
fields involve matrices, so, it is a natural extension to then use these
video cards for other purposes besides graphics. In fact, portfolio
theory in finance is nothing more than a series of matrix operations and
manipulations though we tend to forget this coincidence.

To see matrices in action, let’s remember back to some simple algebra
from high school. Recall the formula for **covariance** in statistics of
two variables:

**σ(a, u) = (a – ā)(u – ū)*****algebraic representation of formula*

Now, let’s represent the same formula in **matrix**
notation[^2^](#sdfootnote2sym):



This type of inverse matrix operation, ***Q*****Λ*****Q’***, is actually
called “**Eigenvalue Decomposition**” and is nothing more than the
transposition of a matrices forming, **Λ**, to produce the covariance or
lambda, **λ**, along the diagonal axis. These types of operations are
used frequently in finance and science so we can see the parallel
between simple algebra and matrix algebra. Just like in graphics
rendering, the GPU is all about matrices so there is a natural
intersection here though not apparent at first glance. So, in hybrid
computing, the computer is simply doing some of the calculations on the
CPU (with larger cores) and others on the GPU (with smaller cores) – by
incorporating pointers and arrays, code can typically be “vectorized” in
terms of executing memory and math operations in “blocks” (versus single
elements) - here is a nice picture:



***Figure 1****–****Hybrid Computing in a Picture****Nvidia,
“uchicago\_intro\_apps\_libs.pdf,” 2012: 21.*

Here is a better layout of one of the most powerful GPU setups in the
world at Oak Ridge National Laboratory, the *Titan*:



***Figure 2****–****Robust GPU Server Cluster****Oak Ridge,
“SC10\_Booth\_Talk\_Bland.pdf,” 2010: 5.*



##Distributed System Summary of GPU Server: 
-----------------------------------------

**Scale:**

 

**Large**

 

 

 

 

 

 

 

 

 

 

 

 

 

**Heterogeneity:**

 

**Radically Diverse**

 

 

 

 

 

 

 

 

 

 

 

 

**Openness:**

 

**Mixed/Challenging - SSH/Tunneling (secure)**

 

 

 

 

 

 

 

 

 

**QOS:**

 

**Mixed/Challenging**

 

 

 

 

 

 

 

 

 

 

 

 

**Storage:**

 

**Distributed File System; DSM (Shared Memory) Async/Sync**

 

 

 

 

 

 

 

 

**Communication:**

 

**IPC - Message Passing**

 

 

 

 

 

 

 

 

 

 

 

**Network:**

 

**Physical Servers; WAN - SSH/Tunneling Access**

 



##Why do we need GPU Computing? Hitting the wall… 
===============================================

As modern scientific research becomes more complex incorporating ever
larger data sets, the classic CPU-centric paradigm runs out of computing
resources. Before GPU computing, programs using intensive computation
might take several days to finish running and calculating the results.
If we were dependent on those results before specifying the next
iteration of a model for testing purposes, we literally had to wait
until proceeding further with research. Think about the wasted time of
such a process. With hybrid computing, we can now get results back in a
matter of hours or even minutes – some firms are even calculating and
executing complex operations in milliseconds/microseconds. These firms
were literally hitting a wall in terms of speed and so needed other
alternatives. GPU computing is massively parallel and scalable and can
speed up applications by 2X to 100X. If you have enough resources and
hardware, you can keep reducing computation time by adding more GPU’s.
This does cost a lot of money, however, and some firms spend tens of
millions of dollars on hardware including both CPU and GPU clusters
(e.g., collocated servers). Remember that a GPU is full of thousands of
smaller cores whereas the CPU has fewer, but larger cores. Think of it
as many little bicycles versus fewer large trucks. However, for certain
tasks, the CPU simply cannot keep up with the GPU. Typically, relative
performance between the CPU and GPU is measured in the amount of
single-and-double precision operations per second (FLOPS). The next
chart shows how fast the divergence in speed is progressing towards a
100X advantage by 2021:



***Figure 3****–****GPU versus CPU****Nvidia, 2012: 6.*



***Figure 4****–****Organizations Using GPU for Speed****Nvidia, 2012:
7.*



***Figure 5****–****Single versus Double Precision Yields More Gains in
Speed****Nvidia, 2012: 8.*


=

***Figure 6****–****Evolution of HPC Networks from CPU to GPU****Oak
Ridge, “SC10\_Booth\_Talk\_Bland.pdf,” 2010: 6.*



##Testing Approach, Setup & Hypothesis 
====================================

The goal is prove out the speed gains from GPU computing by designing
and running a complex program across several platforms. This program
will involve large data sets and complex math operations (see financial
modelling section below for the exact test design). Since this study
involves distributed systems, the program MUST compile and run on a
non-local GPU server cluster (and not simply at home on a single GPU
setup) – getting code to run on foreign, unfamiliar hardware is a
difficult challenge. We will then employ heuristics like timers to
measure the speed across various platforms. To recap, here is a brief
overview of the test:

###[TODO] 
------

1.  *Design a complex program (multi-streams, threads and GPUs) in C/C++
    and make it run on CPU and GPU server (LUC TESLA)*

2.  *Then test variations of the this same program in other
    languages/platforms such as JCUDA, OpenACC/MP & MATLAB [TODO]*

3.  *Keep track of ALL CPU-only and GPU results to determine conclusion
    [TODO]*


 
-

##Description of GPU Server Operations & Architecture 
===================================================



###A CPU Task versus a GPU Task: Pizza Delivery 
--------------------------------------------



CPU cores resemble large trucks compared to smaller GPU cores. Again, it
is helpful to think this dichotomy as trucks versus motorcycles. The CPU
is good for large, sequential operations. Most programs are not
programmed to enable multi-threading on the CPU so it is fair to say
that the CPU goes about its work in non-parallel fashion bouncing around
from task-to-task. On the other hand, the GPU is designed inherently for
parallelism. If we imagine pizza delivery in a neighborhood, the CPU
would deliver a pizza to one house and then move on to the next. The GPU
would send out many smaller messengers simultaneously to multiple houses
via its smaller cores, however, which is why operations must be
**independent**. The next few slides show this pictorially:

***Figure 7****–****A Typical CPU Operation****Nvidia, 2012: 10.*



***Figure 8****–****A Typical GPU Operation****Nvidia, 2012: 11.*

###What does a GPU chip look like? Cores, Caches & SM’s 
----------------------------------------------------

A GPU is largely made up of cores, caches and streaming multiprocessors
(SM’s). Namely, thousands of cores are divided up into blocks on SM’s –
an SM is essentially a mini “brain” on the chip – all the SM’s added
together make up the entire ‘brain’ or chip. Furthermore, each SM has
memory caches and registers. A picture is necessary here – there are 32
cores per SM on this GPU:


***Figure 9****–****GPU Architecture****Nvidia, 2012: 29.*

###Hybrid CPU-GPU Operations: Processing Flow 
------------------------------------------

The next 3 charts demonstrate how the CPU and GPU interact when
processing hybrid functions or operations - CPU contents are copied to
GPU DRAM, the GPU does calculations via SM’s and then GPU contents are
copied back to CPU:


***Figure 10****–****A Typical Hybrid CPU-GPU Operation:
CPU-to-GPU****Nvidia, 2012: 24.*



***Figure 11****–****A Typical Hybrid CPU-GPU Operation: GPU
calculations via SM’s****Nvidia, 2012: 25.*


***Figure 12****–****A Typical Hybrid CPU-GPU Operation:
GPU-to-CPU****Nvidia, 2012: 26.*


 
-


***Figure 14****–****GPU Kernel Execution****Nvidia, 2012: 43.*

###A Simple CUDA C Function 
------------------------

The simple function from Nvidia shows the difference between C code and
CUDA C – CUDA C is just an extension of C with some additional syntax –
the CUDA C compiler is called “NVCC” and is very similar to the standard
C compiler. They are similar except the way they declare and call
functions as well as memory allocation. In CUDA C, you typically use
“**\_global\_ void**” rather than just “**void**” to declare a function.
Also, you must use the triple chevron **“\<\<\< blocks, threads
\>\>\>**” to call a CUDA function. Lastly, you cannot use “**calloc**”
to allocate memory to GPU-related variables – you must use “**malloc**”
instead:



***Figure 15****–****C Function versus CUDA C Function****Nvidia, 2012:
34.*

###A Simple CUDA C Program
-----------------------

In the following CUDA C program, we’re simply going to make a CUDA
function that adds the number 10 to each subsequent iteration of a
variable. We’ll then call that function in the **main** program and
print the results to the console. There are two versions here – one
without error trapping as well as the same version showing how to trap
CUDA errors – it is the same basic program, overall:

***Exhibit 2 – Simple CUDA C Program** Author, 2013.*

\#include \<stdio.h\>



// For the CUDA runtime library/routines (prefixed with "cuda\_") - must
include this file

\#include \<cuda\_runtime.h\>



/\* CUDA Kernel Device code

\* Computes the vector addition of 10 to each iteration i \*/

\_\_global\_\_ void kernelTest(int\* i, int length){



unsigned int tid = blockIdx.x\*blockDim.x + threadIdx.x;



if(tid \< length)

i[tid] = i[tid] + 10;}



/\* This is the main routine which declares and initializes the integer
vector, moves it to the device, launches kernel

\* brings the result vector back to host and dumps it on the console.
\*/

int main(){

//declare pointer and allocate memory for host CPU variable - must use
MALLOC or CudaHostAlloc here

int length = 100;

int\* i = (int\*)malloc(length\*sizeof(int));



//fill CPU variable with values from 1 to 100 via loop

for(int x=0;x\<length;x++)

i[x] = x;



//declare pointer and allocate memory for device GPU variable denoted
with "\_d" – must use cudaMalloc here

int\* i\_d;

cudaMalloc((void\*\*)&i\_d,length\*sizeof(int));



//copy contents of host CPU variable over to GPU variable on GPU device

cudaMemcpy(i\_d, i, length\*sizeof(int), cudaMemcpyHostToDevice);



//designate how many threads and blocks to use on GPU for CUDA function
call/calculation - this depends on each device

dim3 threads; threads.x = 256;

dim3 blocks; blocks.x = (length/threads.x) + 1;



//call CUDA C function - note triple chevron syntax

kernelTest\<\<\<threads,blocks\>\>\>(i\_d,length);

//wait for CUDA C function to finish and then copy results from GPU
variable on device back to CPU variable on host – this is a **blocking**
operation and will wait until GPU has finished calc process

cudaMemcpy(i, i\_d, length\*sizeof(int), cudaMemcpyDeviceToHost);



//print results of CPU variable to console

for(int x=0;x\<length;x++)

printf("%d\\t",i[x]);



//free memory for both CPU and GPU variables/pointers – must use
cudaFree here for GPU variable

free (i); cudaFree (i\_d);



//reset GPU device

cudaDeviceReset(); }



[*https://bitbucket.org/adrew/gpu/commits/7e8154cf89bfc312adbf899187d89622*](https://bitbucket.org/adrew/gpu/commits/7e8154cf89bfc312adbf899187d89622)

##A More Complex CUDA C Program: Model Optimization in Finance 
============================================================

###A Primer in Finance & Building Models – The “Martians” Have Landed 
------------------------------------------------------------------

The CUDA functions and programs demonstrated so far are simple in
nature. This is well and good, but we really want to test how CUDA can
speed up a more complex program. In science and finance, we are often
dealing with models of the world and trying to speed up intensive
computations. To do this, we should build a simple model that has many
iterations and calculations and run it on the CPU (via C) as well as on
the GPU (via CUDA C). We want to determine the benefits of using the
CPU, alone, as well as using the CPU along with the GPU in hybrid
fashion. We will build a trading model in finance in C as well as CUDA
C, so, there will be two versions here. Finance may not be socially
useful, but it is still intellectually challenging since “beating” the
market on a risk-adjusted, consistent basis is very difficult to do
without cheating – we are simply using our brains and probability theory
here instead of relying on “insider” information or a legalized version
of “front-running” (e.g., high-frequency trading via “flash” quotes).
Some enjoy cross-word puzzles while others enjoy finding patterns in the
market.

Let’s take a step back and think about what a ‘model’ is. It is also
helpful to give a brief primer in finance so the reader has some context
as to what we’re striving to accomplish in this demonstration. A model
is nothing more than a simplistic view of the world that describes some
event. The main components of a model are typically referred to as
“**factors**” or variables. That is, we want to try and find factors
that are helpful in describing some state of the world. We could use the
computer to sift through thousands of potential factors and specify the
relevant variables – this is referred to loosely as “machine learning”
(or data mining) and techniques like neural nets (ANN) and regressions
(PLS) are of this type. However, we could also observe the world and
then specify the factors, ourselves, via experience and then test their
usefulness. That is what we’ll do here.

Models are often described in mathematical form. Thinking back to a
regression equation from grammar school, we could have: **y = X \* w +**
….so, ‘**X**’ is the factor or variable here describing ‘y’ while ‘w’ is
the weight or probability of ‘X’. However, we don’t have use math to
describe our model – we could also use logic or conditional language
such as “***IF X THEN Y…***” – this type of language is intuitive and
well-suited to programming since computer code is also written in
pseudo-language. In finance, we typically mean: ***IF CONDITION X THEN
EXCESS RETURNS ON Y*** (e.g., the implicit modeling or prediction of
RETURNS on a stock or security). So, we are “relaxing” math constraints
here and will describe (and program) our model in conditional language.
This “less” precise approach affords us more flexibility to test
“fuzzier” factors we might not have initially envisioned via the strict
math method. In fact, this type of approach is often referred to as
“fuzzy logic” in that it can be “loose” or imprecise.

Fuzzy logic and set theory was pioneered in the last century by a
brilliant thinker named Kurt Gödel[^3^](#sdfootnote3sym). Many brilliant
thinkers emerged out of the collapsed Austro-Hungarian after World War I
including Gödel and it is important to mention them since most of the
tools we use today were created by them – we owe a collective “tip of
the hat” to this group. These thinkers laid the foundation for modern
math and physics as well as computers. Humorously, this group made up of
the likes of John Von Neumann, Edward Teller and Gödel were often
referred to as the “Martians” since they were considered so smart that
it was if they came from another planet[^4^](#sdfootnote4sym). It could
be argued that modern generations have not come as far since we have not
developed new fields in math or science though we have more to go on via
computing power. Remember also that the last large space operation
occurred in the 1960’s when von Braun and his rocket scientists sent us
to the moon – a remarkable feat considering the limited computing power
at the time compared to nowadays.

In the 1931, Gödel published a set of logical theories including the
“**incompleteness theorem**”. At the time, it was trendy for both
mathematicians and physicists to tinker in philosophy. In fact, thinkers
like Bertrand Russell at Cambridge University were actually trying to
make philosophy a science by applying various rigorous disciplines.
Namely, Russell and several others were trying to create a “perfect”
human language with no ambiguities based on Boolean logic and
mathematics[^5^](#sdfootnote5sym). The idea was that one could represent
linguistic expressions as mathematical conditions with binary (e.g.,
true-or-false) outcomes and, therefore, switch back-and-forth between
the two modes of expression. Today, this endeavor might seem trite or
foolish, but the hope was that a clear language could rid the world of
misunderstanding and suffering. A mathematician, himself, Gödel came
along and “wrecked” this logical quest though Wittgenstein also made
some contributions to the cause. With the “incompleteness theorem”,
Gödel demonstrated how a mathematical function can be logically
constructed in language and vice-versa, but still remain improvable
though simultaneously consistent or rational. So, he demonstrated how
something logical could still end up being circular or incomplete in
some sense (e.g., always lacking or imperfect). The long, formal proof
of this theorem is beyond the scope of this study, but let it suffice
that the two main points are that if a system is consistent or logical,
then it cannot be complete and its axioms cannot be proven within the
system. With one master stroke, Gödel basically pushed “back” math,
physics and philosophy into “grey” space despite the best efforts of
thinkers like Russell to make these disciplines binary or
“black-and-white”. So, Gödel brought back ambiguity and made it “okay”,
so to speak, since he showed that statements that are improvable can
still have meaning or consistency. Since he used both math and language
to illustrate his proof, his findings could not be refuted by
mathematicians and philosophers, alike, which is why they had such
far-reaching implications.

This theme of “greyness” or fuzziness is important because it relates
back to our model in finance. Loosely speaking, the word “fuzzy”
basically implies that something has meaning or usefulness though it is
ambiguous. Mapping this theme into logic or probability theory, “fuzzy”
means that something is not **true** or **false** in a strict sense, but
rather has a degree of truth or consistency between **0** and **1**. So,
a statement has a probability or outcome expressed as an interval or
percentage between the binary levels of 0 and 1. Essentially, fuzzy
logic implies a lack of precision and is a matter of degree or range,
instead, which is very relevant to human language via conditional
statements. It is as if to admit that there is only a “sense” of the
truth in human affairs which are often complex. Imagine someone making a
stock market prediction and framing it as a matter of probability rather
than certainty which seems prudent, since, it is so difficult to make
prognostications about the future. A typical fuzzy statement is the
following: “***Unexpected results of government reports cause big moves
in the stock market***”. Intuitively, this statement seems somewhat true
or meaningful though it is not exactly quantified and, hence, ambiguous.
This is the type of expression or outcome Gödel was trying to describe
in his proof. Now imagine a computer trying to process the statements
“cold, colder, coldest”. Though these words have some “rough” meaning to
humans, computers cannot quantify these expressions unless a range of
temperatures (e.g., a “fuzzy” interval) describing each subset or word
is also supplied. More formally in 1965, Lotfi Zadeh mathematically
described a “fuzzy” set as a pair (**A,m**) where **A** is a set and
**m** : **A********[0,1][^6^](#sdfootnote6sym)**. A fuzzy set or
interval of a continuous function can also be written: A \* R. In terms
of modeling, it’s more useful to think of a fuzzy set in the form
**A****●****R = B** where A and B are fuzzy sets and **R** is a fuzzy
“relation” – A ● R stands for the composition A with
R[^7^](#sdfootnote7sym). So, what does this mean? It means that though
sets A and B are originally independent, they might be probabilistically
connected through **R**, the fuzzy relation, by one element in both
sets. It is like saying A and B are independent, but that they could
also be probabilistically related to each other via R – there is a sense
of “fuzziness” or ambiguity here.

Now we have fuzzy logic under our belts, let’s go ahead and specify our
fuzzy model. Note that we will not formally or explicitly define our
model and its factors via math, but do this implicitly, instead, via
language. From observations and experience, we know that financial
markets get interesting at the extremes. What does “extreme” mean? In
terms of price action, this idea pertains to when markets get
**overbought** (e.g., price has risen very high) or **oversold** (e.g.,
price has dropped very low). So, we want to build a simple model that
can give us some insight into what happens when prices reach extremes.
Should we buy on strength or “momentum” when prices are very high and
overbought? Or, should we sell short in a kind of counter-trend or
**mean-reversion** trade? Note that this relates to human behavior to
some degree since people often engage in “crowding” actions akin to
“fear-and-greed” cycles. So, this model will also have a behavioral edge
as well as underpinnings to fuzzy logic.

To discover more, we must first find a way to measure extremes. From
experience, we will use a statistical “trick” or tool called
“**Z-scores**”. A z-score is simply defined as: **z = (a – ā)/σ**. So,
take today’s price for a security and subtract from it a moving-average
price and then divide by the standard deviation (or volatility) of the
time series. At any given time, this measure will tell us how many
standard deviations “**rich**” or “**cheap**” the current price is when
compared to the average price. Sounds simple, but remember that we
already have two parameters of this function – a parameter is simply an
input into a function. Namely, we have **length** and a suitable cutoff
level or fuzzy **range** that defines ‘rich’ or ‘cheap’. So, our trading
rules becomes: **IF Z-SCORE TODAY IS \>= Z-SCORE RANGE THEN BUY** (and
vice-versa for SELL trades). From testing and experience, we happen to
know that “going with the market” (instead of fighting it) yields better
results – so, this type of model will be a **momentum** model since we
are simply reacting to and ‘going with’ the recent trend in prices.
Notice also that we only care about price here – we don’t care about
**exogenous**, fundamental variables like GDP (Gross Domestic Product).
So, we are **keying** off “price action” to make a formulation about
trading on the price of a security – there are no abstract levels of
determination here since we are studying price to take action on price,
itself. This type of model is often referred to as a technical or
price-based model and is **endogenous**.

So, we have a relatively simple model here with one implicit factor
called z-score. However, we’ll need to test various values to determine
the optimal parameters of the model – length and z-score range. This
will involve combinatorial math. Let’s say we have 3 possible inputs for
these 2 parameters: {**1, 2, 3**} for z-score ranges and {**21, 34,
55**} for moving-average lengths – that is **3**^**2**^ combinations or
9 in total. Models can often have 2, 3 or even 4 factors – so we could
easily have **3**^**4**^or 81 combinations for testing. This means for
each security, we must test **all** combinations over each time period
to find the best one. This is called “**exhaustive** optimization” since
we are testing all combinations – **genetic**optimization is faster, but
there is no guarantee of finding the best result. What is the ‘**best**’
one? In finance, this is often called lambda, **λ**, or the Sharpe
ratio: **μ/****σ** (the average return over the time period divided by
the volatility over the same time period). This is not to be confused
with lambda or “half-life” from physics. There is an old joke that if
your math is not good enough, you leave science and go into finance. All
jokes aside, finance does indeed borrow a lot from physics – delta,
gamma, lambda and omega are all used in option trading besides the fact
that the Black-Scholes pricing equation (for options) was derived from
the heat-transfer equation in physics (e.g., Ito’s Lemma). So, we’ll
have to run all these combinations and then store the best combination
for use later in the model for each security. So, 30 securities (though
we could easily have more) multiplied by 9 combinations is a total of
270 optimizations. We can begin to see that the amount of computation is
piling up here.

To add more realism to this model testing, we’ll also have to repeat
this optimization over-and-over for each time period on each security.
So, as we move through time, our optimization process becomes a
“rolling” optimization. We must look “back” and test the best possible
parameter combination and then use that one for the next period forward
to derive unbiased model returns (e.g., “profit-and-loss”) for each
security. The “look back” period is often referred to as an
“**in-sample**” test while the next one forward is called an
“**out-of-sample**” test (e.g., “ex-ante” versus “ex-post”). Remember
that once we’ve found the best parameter combination on a security, we
cannot go back in a time machine and trade off these results – so, we’ll
simply hold these parameters constant and use them for the next period
forward in terms of trading and results. Since this model will be
re-optimizing and **updating** with the market, it is also a kind of
learning model or **DLM** (Dynamic Learning Model). The world of finance
has been waiting patiently for the physicists to find the “time machine”
model, but nothing has been found as of yet though quantum theory
suggests this might be possible. This concept can be confusing and is a
common mistake amongst modelers so let’s draw a timeline depicting this
**rolling** optimization – here we are fitting parameters (for z-score)
on the last 5 years and then repeating this process every year (or
re-optimizing in **steps** of 1):

***Exhibit 4 – Rolling Optimization: In-Sample (Blue) vs. Out-of-Sample
(Black)** Author, 2013.*



Considering many financial time series (in the futures markets) have an
average length of at least 39 years, than mean 35 in-sample
optimizations in total. So, now we have [**35 periods \* 9 combinations
\* 30 securities**] equals **9,450** optimizations/combinations in
total. Even though we have a simple 1-factor model, the amount of
calculations increases exponentially. A more complex model could have
[**35 periods \* 81 combinations \* 30 securities**] equals **85,050**
optimizations/combinations in total. This amount of computation will
easily overwhelm any CPU (including quad-cores) on a workstation or
desktop which is really why we need GPU’s here as well as multiple
workstations (and GPU’s). In the real world, no model is perfect since
it is merely a simple representation of the world. Therefore, it is
common in finance to run multiple models **simultaneously** in order to
diversify the portfolio in an extreme sense. Instead of diversifying a
portfolio of stocks according to finance theory, take this theme a level
**higher** and imagine becoming a portfolio manager of models (instead
of just securities). Now envision this: [**35 periods \* 81 combinations
\* 30 securities \* 30 models**] = **2,551,500**
optimizations/combinations in total. Typically, re-optimization is done
every week instead of every year like the example above (e.g., **smaller
steps**) – *Houston we have a problem*! Hopefully, everyone gets the
gist of the computational challenges in the world of finance and
science.


 
-

##Using Wrappers and Other “Non-native” Packages for Modelling: What are the tradeoffs of using these alternatives? 
=================================================================================================================

###The MATLAB Stats package 
------------------------

Instead of writing CUDA on the lowest, most pure level (e.g., **kernel**
level), we can use “**wrappers**” in other programming packages,
instead. Wrappers are overlays or translators that call more complex
functions beneath them allowing for easier use and operability.
Statistics packages like MATLAB give us this option and are forgiving in
that they are “high-level” packages in which a few keystrokes can result
in powerful results. Essentially, packages like MATLAB do a lot of the
heavy lifting for us and so have become very popular within science and
finance. Cleve Moler originally created MATLAB in the 1970’s based on
FORTAN and then morphed into C later on. MATLAB stands for “MATRIX
LABORATORY” and it is a very powerful stats program that is optimized
for large data sets and vectorization of code. MATLAB is not a native
language like C (though there is a MATLAB C compiler that can transform
“M-code” into native C), but it is still very fast – unfortunately, it
is not open source and is expensive (unlike packages like R). More
precisely, MATLAB is more like a functional language and is fourth
generation type in nature[^8^](#sdfootnote8sym). In terms of CUDA, we
will take a slight hit to performance here since we are one level
removed from purity, so to speak. There is also less functionality
compared to the standard CUDA library. Here is what the M-code looks
like in terms of the main part of the original model program:

###[TODO] 
------

***Exhibit 7 – A Complex MATLAB Program w/Wrappers: Trading Model**
Author, 2013.*



Again, the performance hit will be notable running GPU operations in
MATLAB versus CUDA C – it is roughly **2X** to **4X** slower in MATLAB.
Also, note that open source statistics packages also exist such as R and
SPYDER. Each package handles GPU functionality in its own way (if
available). For PYTHON lovers, a great package is called SPYDER that
comes with an extension called PYCUDA. This combination is the closest
thing to MATLAB:
[https://code.google.com/p/spyderlib/](https://code.google.com/p/spyderlib/)
.

###JCUDA 
-----

Because JAVA is still so popular and is often used in HPC open-source
computing, it is an interesting challenge see how well it can execute
the same complex test program by wrapping CUDA functions (hence
“JCUDA”). Since we’ve already installed the drivers for earlier
programs, we simply need to add the binary packages from the JCUDA
website and place these in our JAVA PATH folder:
[http://www.jcuda.org/tutorial/TutorialIndex.html](http://www.jcuda.org/tutorial/TutorialIndex.html)
. If all goes well, we should then be able to compile the same program
after changing some pointer references and see something like this:


###[TODO] 
------

##Extreme GPU Computing: Master Level Complexity 
==============================================

###Using Multiple GPU’s (Server/Cluster) & Streams for Further Speed Gains in Model Program – Multi-GPU’s & Multi-Streams (Plus Advanced Math Functions) 
-----------------------------------------------------------------------------------------------------------------------------------------------------

We’ve been building up in complexity in terms of GPU programs. Finally,
we’ve arrived at the most complex iteration possible and it involved
CUDA C (which should come as no surprise). Previously, we only ran one
CUDA kernel at a time on one GPU device – recall that NVIDIA’s PROFILER
tool suggested that CONCURRENCY was non-existent (see page 39). So, even
though we were multi-threading in earlier programs, **there is more to
true parallelism than threading**. For maximum speed gains, we’re now
going to fix that as well as other adjustments (e.g., more advanced math
functions plus multiple GPU devices).

When we have access to a GPU cluster or **server**, we can get further
speed gains by re-writing our code (in LINUX typically since most
servers run on this OS). Recall in the earlier example of the model
program, we were only looping through **1** market (or security) at a
time. Though the CUDA function was running in parallel by using multiple
cores (for multiple threads) simultaneously to generate trade positions
and returns, there is still a bit of a serial or sequential “feel” here
in terms of process. But, if we have a large enough GPU (e.g., a TESLA
GPU) or even multiple GPU’s, we can re-write our code to process several
markets or securities at once. For example, NVIDIA[^9^](#sdfootnote9sym)
has 2 TESLA (K20) GPU’s on one of its server nodes. So, theoretically,
we could at least process 2 markets simultaneously instead of stepping
through **market-by-market**. Theoretically, this should further reduce
computational time by a factor of 2, but it does not exactly work out
this way due to additional overhead regarding the coordination of
multiple GPU’s.

###Amdhal’s Law 
------------

Let’s remember **AMDHAL**’s law: **S = 1 / 1−P**. Now, if a program is
further parallelized, the maximum speed-up over serial code is **1 / (1
– 0.50) = 2**. So, we should still see additional speed gains overall
though not by a factor of 2 exactly. Here is what the main part of the
code looks like in terms of multi-streaming and multiple GPU’s –
remember that we had 4 nested “**for-next**” loops earlier (see pages
34-37) – now we will need a total of **5** loops: **LOOP BY GPU DEVICE
NUMBER \>\>\> LOOP BY MARKET \>\>\> LOOP BY TIME PERIOD \>\>\> LOOP BY
FIRST PARAMETER \>\>\> LOOP BY SECOND PARAMETER:**



So, how did we do? We now have the **fastest**computational times yet:
1810 optimization in 65 seconds (or **0.036** seconds per optimization).
All that work finally paid off since we were at 87 seconds before
running on a single GPU without multiple streams – so, roughly a +33%
pickup in speed. Here is a fun picture of a massive server room at a
local business (name withheld):



=



##Conclusion and Wrap-Up: Did we prove out speed gains? 
=====================================================

We can see that GPU computing is a powerful tool and so it is important
to demonstrate the benefits of the inherent speed gains. Open source
libraries like OpenACC can also be useful while wrapper programs like
MATLAB/JCUDA are slower. Lastly, this survey should demonstrate that GPU
computing need not be all that complicated – truly, we don’t have to be
geniuses to take advantage of hybrid HPC and younger generations are
encouraged to get involved, especially, as the world moves from finance
back to “hard” science.

###[TODO] 
------

***Exhibit 9 – Trading Model Performance Results Across Platforms**
Author, 2013.*



***Figure 19****–****Organizations Using GPU Accelerators****Nvidia,
2012: 45.*


=


##Bibliography 
============



Cochrane, John, *“Time Series for Macroeconomics and Finance,”*
University of Chicago, January 2005.



Coulouris, George, “*Distributed Systems and Concepts, Fifth Edition*,”
Addison-Wesley, 2012.



*Fuzzy set*. (n.d.). Retrieved February 2013, from Wiki:
[http://en.wikipedia.org/wiki/Fuzzy\_set](http://en.wikipedia.org/wiki/Fuzzy_set)



Gleick, James, “*The Information: A History, a Theory, a Flood*,”
Pantheon, 2011.



Hargitta, Istran. *Martians of Science*. New York, NY: Oxford Press,
2006.



Kochan, Stephen, “*Programming in C, Third Edition*,” Sams Publishing,
July 2005.



*Matlab*. (n.d.). Retrieved February 2013, from Wiki:
http://en.wikipedia.org/wiki/Matlab



Nvidia, “*CUDA C Guide and Best Practices*,” Nvidia, 2013.



*Nvidia Cuda*. (n.d.). Retrieved February 2013, from Wiki:
http://en.wikipedia.org/wiki/CUDA



Thiruvathukal, George, “*High-Performance Java Platform Computing*,”
Pearson Education, 2000.



[1](#sdfootnote1anc)Feb. 2012 \<
[http://www.nvidia.com/object/cuda\_home\_new.html](http://www.nvidia.com/object/cuda_home_new.html)
\>.

[2](#sdfootnote2anc)John Cochrane, “Time Series for Macroeconomics and
Finance,” University of Chicago January 2005: 262.

[3](#sdfootnote3anc)James Gleick, The Information: A History, a Theory,
a Flood (New York: Pantheon, 2011) 136-186.

[4](#sdfootnote4anc)Istran Hargitta, Martians of Science (New York:
Oxford Press, 2006) 11.

[5](#sdfootnote5anc)Gleick 136-186.

[6](#sdfootnote6anc)Feb. 2013 \<
[http://en.wikipedia.org/wiki/Fuzzy\_set](http://en.wikipedia.org/wiki/Fuzzy_set)
\>.

[7](#sdfootnote7anc)Feb. 2013 \<
[http://en.wikipedia.org/wiki/Fuzzy\_set](http://en.wikipedia.org/wiki/Fuzzy_set)
\>.

[8](#sdfootnote8anc)Feb. 2013 \<
[http://en.wikipedia.org/wiki/Matlab](http://en.wikipedia.org/wiki/Matlab)
\>.

[9](#sdfootnote9anc)The numerical simulations needed for this work were
performed on [NVIDIA Partner] Microway's Tesla GPU accelerated compute
cluster.