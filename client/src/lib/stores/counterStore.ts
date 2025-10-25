import { makeAutoObservable } from "mobx";

export default class CounterStore {
  title = "Counter Store";
  count = 0;
  events: String[] = [`Initial count is ${this.count}`];

  constructor() {
    // makeObservable(this, {
    //   title: observable,
    //   count: observable,
    //   increment: action,
    //   decrement: action
    // });
    makeAutoObservable(this);
  }

  increment = (amount = 1) => {
    this.count += amount;
    this.events.push(`Incremented by ${amount} - count is now ${this.count}`);
  };

  decrement = (amount = 1) => {
    this.count -= amount;
    this.events.push(`Decremented by ${amount} - count is now ${this.count}`);
  };

  get eventCount() {
    return this.events.length;
  }
}
