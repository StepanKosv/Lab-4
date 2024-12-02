
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
//using Variant=(Program.Condition condition,Program.Option option);

internal class Program
{
    static bool isListCreated=false;
    static int[] list;
    static bool isListSorted=false;
    static int maxListLenght=1000;
    static int minRandomElement=-1000;
    static int maxRandomElement=1000;
    static Random random=new Random();
    /*
    static Variant listNotCreatedError=(
        ()=>!isListCreated,
        ()=>Console.WriteLine("ошибка: вы не создали список")
    );
    static Variant listIsEmptyError=(
        ()=>list.Length==0,
        ()=>Console.WriteLine("ошибка: минимум отсутствует(пустой список)")
    );
    */
    
    //public delegate bool Condition();
    public delegate void Option();
    /*
    static void RunCommand((Condition condition,Option option)[] errors, Option defaultOpt){
        foreach (var pair in errors){
            if(pair.condition()){
                pair.option();
                return;
            }
        }
        defaultOpt();
    }
    */
    static void RunOnList(Option opt){
        if(!isListCreated){
            Console.WriteLine("ошибка: вы не создали список");
            return;
        }
        opt();
    }
    static void RunOnNonEmptyList(Option opt){
        RunOnList(
            ()=>{
                if(list.Length==0){
                    Console.WriteLine("ошибка: список не содержит не одного элемента");
                    return;
                }
                opt();
            }
        );
    }
    static void RunSorting(Option sorting){
        RunOnList(
            ()=>{
                sorting();
                isListSorted=true;
            }
        );
    }
    static void SortByChose(){
        int end=list.Length-1;
        while(end>0){
            int maxi=0;
            for(int i=0; i<=end; i++){
                if(list[maxi]<list[i]){
                    maxi=i;
                }
            }
            int d=list[maxi];
            list[maxi]=list[end];
            list[end]=d;
            end--;
        }
        isListSorted=true;
        Console.WriteLine("сортировка выполнена успешно");
    }
    static int[] Merge(int[] a, int[] b){
        int[] res=new int[a.Length+b.Length];
        int ai=0,bi=0;
        for(int i=0;i<res.Length;i++){
            if(ai>=a.Length||(bi<b.Length&&b[bi]<a[ai])){res[i]=b[bi];bi++;}
            else{res[i]=a[ai];ai++;}
        }
        return res;
    }
    static int[] MergeSort(int[] l){
        if(l.Length<=1) return l;
        return Merge(MergeSort(l.Where((a,i)=>i<l.Length/2).ToArray()),MergeSort(l.Where((a,i)=>i>=l.Length/2).ToArray()));
    }
    static void QuickSort(int[] arr, int left, int right){
        if(right-left<=1) return;
        int mid=arr[random.Next(left,right-1)];
        int l=left,r=right;
        for(int i=l;i<r;i++){
            if(arr[i]<mid){
                (arr[l],arr[i])=(arr[i],arr[l]);
                l++;
            }
            else if(arr[i]>mid){
                (arr[r-1],arr[i])=(arr[i],arr[r-1]);
                r--;
                i--;
            }
        }
        QuickSort(arr,left,arr.Select((a,i)=>i).First((i)=>arr[i]==mid));
        QuickSort(arr,arr.Select((a,i)=>i).Last((i)=>arr[i]==mid)+1,right);
    }
    static int InputLenght(int max){
        bool isCorrect=false;
        int res;
        Console.WriteLine($"введите число элементов(не больше {max})");
        do{
            if((!int.TryParse(Console.ReadLine(),out res))||res<0){
                Console.WriteLine("ошибка: список содержит целое неотрицательное количество элемнтов ");
            }else if(res>max){
                Console.WriteLine("ошибка: слишком большое количество элементов");
            }else{
                isCorrect=true;
            }
        }while(!isCorrect);
        return res;
    }
    static int InputElement(){
        int res=0;
        Console.WriteLine("Введите элемент");
        while(!int.TryParse(Console.ReadLine(),out res)){
            Console.WriteLine("ошибка: элемент должен быть числом в пределах int");
        }
        return res;
    }
    static void InputList(Func<int> inpEl){
        int n=InputLenght(maxListLenght);
        list=new int[n];
        for(int i=0;i<n;i++){
            list[i]=inpEl();
        }
        isListSorted=false;
        isListCreated=true;
        Console.WriteLine("Список сформирован успешно");
    }
    static void PrintList(){
        Console.WriteLine(String.Join(" ",list));
    }
    static int InputPosition(){
        int max=list.Length+1;
        bool isCorrect=false;
        int res;
        Console.WriteLine($"введите позицию вставки(не больше {max})");
        do{
            if((!int.TryParse(Console.ReadLine(),out res))||res<=0){
                Console.WriteLine("ошибка: позиция вставки должна быть целым положительным числом");
            }else if(res>max){
                Console.WriteLine("ошибка: позиция вставки выходит за границы списка");
            }else{
                isCorrect=true;
            }
        }while(!isCorrect);
        return res;
    }
    static void InsertElements(){
        int n=InputLenght(maxListLenght-list.Length);
        int k=InputPosition()-1;
        int[] newlist=new int[list.Length+n];
        for(int i=0;i<list.Length+n;i++){
            if(k<=i && i<k+n){
                newlist[i]=InputElement();
            }else if(i>=k+n){
                newlist[i]=list[i-n];
            }else{
                newlist[i]=list[i];
            }
        }
        list=newlist;
        isListSorted=false;
        Console.WriteLine("Вставка элементов произведена успешно");
    }
    static void Swap(){
        Console.WriteLine("Четные и нечетные элементы поменяны местами");
        for(int i=0;i<list.Length/2;i++){
            int d=list[2*i];
            list[2*i]=list[2*i+1];
            list[2*i+1]=d;
        }
        if(list.Length%2==1){
            Console.WriteLine("Всего элементов нечетное количество. Последний элемент оставлен на своем месте");
        }
    }
    static void RemoveMin(){
        int min=list.Min();
        int k=list.Count((e)=>e==min);
        int[] pos=list.Select((e,i)=>i+1).Where((e,i)=>list[i]==min).ToArray();
        list=list.Where((e,i)=>pos.First()!=i+1).ToArray();
        Console.WriteLine($"значение минимума:{min} его позиции:{String.Join(' ',pos)}. удалено первое вхождение минимума");
    }
    static int BinSearch(int key,int l, int r, Func<int,int,bool> comp,ref int k){
        k+=1;
        int m=(l+r)/2;
        if(r-l<=1) return l;
        if(comp(list[m],key)){
            return BinSearch(key,l,m+1,comp,ref k);
        }
        return BinSearch(key,m,r,comp,ref k);
    }
    static int BinSearchLeft(int key,int l, int r, ref int k){
        k+=1;
        int m=(l+r)/2;
        if(r-l<=1) return r;
        if(list[m]>=key){
            return BinSearchLeft(key,l,m,ref k);
        }
        return BinSearchLeft(key,m,r,ref k);
    }
    static int BinSearchRight(int key,int l, int r, ref int k){
        k+=1;
        int m=(l+r)/2;
        if(r-l<=1) return l;
        if(list[m]>key){
            return BinSearchRight(key,l,m,ref k);
        }
        return BinSearchRight(key,m,r,ref k);
    }
    static void FindElement(){
        int key=InputElement();
        if(isListSorted){
            int k=0;
            int start=BinSearchLeft(key,-1,list.Length-1,ref k);
            int end=BinSearchRight(key,0,list.Length,ref k);
            if(list[start]!=key) Console.WriteLine($"элемент отсутствует. количество действий:{k}");
            else Console.WriteLine($"массив отсортирован в порядке возрастания, позиции элемента:{start+1}-{end+1}. количество сравнений:{k}");
        }else{
            int k=0;
            int c=0;
            foreach(int i in list){
                if (i==key)c+=1;
                k++;
            }
            if (c==0) Console.WriteLine($"элемент отсутствует. количество действий:{k}");
            else{
                var pos=new int[c];
                int ind=0;
                for(int i=0;i<list.Length;i++){
                    if(list[i]==key){
                        pos[ind]=i+1;
                        ind++;
                    }
                    k++;
                }
                Console.WriteLine($"массив не отсортирован, позиции элемента:{String.Join(' ',pos)}. количество сравнений:{k}");
            }
        }
    }
    static List<int> BucketStort(List<int> arr, int min, int max){
        if(arr.Count<=1){
            return arr;
        };
        List<int> res=new List<int>();
        int k=arr.Count;
        double dist=max-min;
        double step=dist/k;
        List<List<int>> buckets=new List<List<int>>();
        for(int i=0;i<k+1;i++) buckets.Add(new List<int>());
        foreach(int e in arr){
            int i=(int)((e-min)/step);
            buckets[i].Add(e);
        }
        foreach(List<int> e in buckets)res.AddRange(BucketStort(e,e.Min(),e.Max()));
        return res;
    }

    private static void Main(string[] args)
    {
        bool end=false;
        string menu="Меню"+'\n'+
        "Введите номер действия"+'\n'+
        "Возможные действия:"+'\n'+
        "-1. Вывести меню на экран"+'\n'+
        "0. Выйти из меню и завершить работу"+'\n'+
        "1. Сформировать массив из n элементов: ввести с клавиатуры"+'\n'+
        "2. Сформировать массив из n элементов с помощью датчика случайных чисел"+'\n'+
        "3. Распечатать массив"+'\n'+
        "4. Удалить первый минимум из массива"+'\n'+
        "5. Добавить N элементов, начиная с номера К"+'\n'+
        "6. Поменять местами элементы с четными и нечетными номерами "+'\n'+
        "(в случае если элементов нечетное количество, последний останется на месте)"+'\n'+
        "7. Выполнить поиск элемента в массиве и подсчитать количество сравнений"+'\n'+
        "(В отсортированном массиве будет использован бинарный поиск)"+'\n'+
        "8. Выполнить сортировку массива простым выбором"+'\n'+
        "9. Выполнить сортировку массива слиянием"+'\n'+
        "10. Выполнить сортировку массива быстрой сортировкой"
        ;
        Console.WriteLine(menu);
        do{
            int option;
            Console.WriteLine("Введите целое число - id пункта в меню");
            while(!int.TryParse(Console.ReadLine(),out option)){
                Console.WriteLine("Ошибка: команда должна быть целым числом - номером пункта в меню");
            }
            switch(option){
                case -1:
                    Console.WriteLine(menu);
                    break;
                case 0:
                    Console.WriteLine("Программа завершила работу");
                    end=true;
                    break;
                case 1:
                    InputList(InputElement);
                    break;
                case 2:
                    InputList(()=>random.Next(minRandomElement,maxRandomElement));
                    break;
                case 3:
                    RunOnList(PrintList);
                    break;
                case 4:
                    RunOnNonEmptyList(RemoveMin);
                    break;
                case 5:
                    RunOnList(InsertElements);
                    break;
                case 6:
                    RunOnList(Swap);
                    break;
                case 7:
                    RunOnNonEmptyList(FindElement);
                    break;
                case 8:
                    RunSorting(SortByChose);
                    break;
                case 9:
                    RunSorting(()=>{list=MergeSort(list);Console.WriteLine("сортировка выполнена успешно");});
                    break;
                case 10:
                    RunSorting(()=>{QuickSort(list,0,list.Length);Console.WriteLine("сортировка выполнена успешно");});
                    break;
                default:
                    Console.WriteLine("ошибка: пункт отсутствует в меню. Введите '-1' чтоб просмотреть меню");
                    break;
            }
        }while(!end);
    }
}
