from itertools import combinations
import numpy as np
from functools import reduce
from collections import defaultdict
from time import time


def generate_cubes(letters, number_of_cubes, max_letter_repetitions=4, cube_sides=6):
    """
    Generates set of required number of cubes with limit to letter repetitions on cubes.
    Warning - will generate without stop!
    """
    while True:
        letters_pool = letters * max_letter_repetitions
        cubes = []
        for _ in range(number_of_cubes):
            cube = []
            while len(cube) < cube_sides:
                letter = letters_pool[np.random.randint(0, len(letters_pool) - 1, 1)[0]]
                if letter not in cube:
                    cube.append(letter)
                    del letters_pool[letters_pool.index(letter)]
            cubes.append(cube)
        yield cubes


def check_can_write_with_set(word, cubes_set):
    """checks if word can be written with given cubes set"""

    def find_cubes_with_char():
        ix = []
        for i, cube in enumerate(cubes_set):
            if char in cube:
                ix.append(i)
        return ix

    cubes_with_char = {}
    for char in set(word):
        cubes_ix = find_cubes_with_char()
        if len(cubes_ix) == 0:
            return False
        cubes_with_char[char] = cubes_ix

    selected_cubes = set(reduce(lambda a, b: a + b, cubes_with_char.values()))
    if len(word) > len(selected_cubes):
        return False

    sorted_word = sorted(word, key=lambda char: len(cubes_with_char[char]))
    for char in sorted_word:
        ix = cubes_with_char[char]
        no_cube_for_letter_flag = True
        for i in ix:
            if i in selected_cubes:
                selected_cubes.remove(i)
                no_cube_for_letter_flag = False
                break
        if no_cube_for_letter_flag:
            return False

    return True


def search_for_best_cube_set(words, number_of_cubes=10, max_letter_repetitions=4):
    """
    Searches and prints cube sets with increasing score.
    Score is calculated based on how many words can be written using this cube set.
    It never stops, so run should be interrupted.
    """
    letters = "абвгдеёжзийклмнопрстуфхцчшщьыъэюя"  # available chars
    letters = [letter for letter in letters if letter not in {"х", "ж", "ъ", "ё"}]  # excluding some chars
    print(f"{letters=}")
    all_possible_cubes = np.array([*combinations(letters, 6)])
    print((len(all_possible_cubes)))

    start_time = time()
    words_scores = defaultdict(int)
    counter = 0
    best_score = 0
    for cube_set in generate_cubes(letters, number_of_cubes, max_letter_repetitions):
        counter += 1
        set_score = 0
        offset = len(words) - best_score
        for i, word in enumerate(words):
            check = check_can_write_with_set(word, cube_set)
            set_score += 1 if check else 0
            words_scores[word] += 1 if check else 0
            if set_score <= i - offset:
                set_score = -1
                break
        if set_score >= best_score:
            print(f"Cube found with score {set_score:0d}")
            print(np.array(cube_set))
            best_score = set_score
        if counter % 10_000 == 0:
            print(f"{counter / 10_000 :.0f}0k evaluated in {(time() - start_time) / 60:.1f} mins.")


if __name__ == '__main__':
    words = ['мандарин', 'снеговик', 'вьюга', 'оливье', 'елка', 'звезда', 'салют', 'фейерверк', 'куранты', 'шарики',
             'подарки', 'мороз', 'снегопад', 'метель', 'санки', 'аргомак', 'ледянка', 'коньки', 'письмо', 'конфеты',
             'бык', 'мороз', 'зима', 'сосулька', 'лед', 'сноуборд', 'декабрь', 'январь', 'февраль', 'шуба', 'батарея',
             'сугроб', 'снегурочка', 'горка', 'каникулы', 'шапка', 'перчатки', 'лопата', 'гололед', 'валенки',
             'снегирь', 'радость', 'скользко', 'темно', 'спячка', 'берлога', 'буря', 'крещение', 'узор']

    words = sorted(words, key=lambda x: len(x), reverse=True)

    search_for_best_cube_set(words)
