using System;
    public class Node<T> {
 
        public Interval range;
        public Node<T> left, right;
        public T value;
        public double max;

        public T Value { get; set;}
        public Node(Interval range, double max, T value)
        {
            this.range = range;
            this.max = max;
            this.value = value;
        }
 
        public override string ToString()
        {
            return "[" + this.range.low + ", "
                + this.range.high + "] "
                + "max = " + this.max + "\n";
        }
    }
    public class Interval {
 
        public double low, high;
 
        public Interval(double low, double high)
        {
            this.low = low;
            this.high = high;
        }
 
        public override string ToString()
        {
            return "[" + this.low + "," + this.high + "]";
        }
    }
 
public class IntervalTree<T> {

    public static Node<T> insert(Node<T> root, Interval x, T value)
    {
        if (root == null) {
            return new Node<T>(x, x.high, value);
        }
        if (x.low < root.range.low) {
            root.left = insert(root.left, x, value);
        }
        else {
            root.right = insert(root.right, x, value);
        }
        if (root.max < x.high) {
            root.max = x.high;
        }
        return root;
    }
    public static void inOrder(Node<T> root)
    {
        if (root == null) {
            return;
        }
        inOrder(root.left);
        Console.Write(root);
        inOrder(root.right);
    }
    public static T isOverlapping(Node<T> root,
                                         double x)
    {
        if (root == null) {
            // return a dummy interval range
            return default(T);
        }
        // if x overlaps with root's interval
        if ((x > root.range.low
             && x < root.range.high)) {
            return root.value;
        }
        else if (root.left != null
                 && root.left.max > x) {
            // the overlapping node may be present in left
            // child
            return isOverlapping(root.left, x);
        }
        else {
            return isOverlapping(root.right, x);
        }
    }
}