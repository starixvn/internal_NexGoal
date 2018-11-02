import React from 'react';
import PersonalTimeline from '../timeline/PersonalTimeline';
import TimelineNode from '../timeline/TimelineNode';
import renderer from 'react-test-renderer';
import { shallow } from 'enzyme';

const records = [{
    performanceRecordId: 1,
    gradeId: 2,
    grade: { name: '' },
    createdDate: '',
    employee: { nickName: 'Oliver' },
    creator: { nickName: 'Michael', avatar: '' }
}];

var data = {
    prevDisabled: true,
    nextDisabled: false,
    records: records,
    pageIndex: 2
};

describe('Test timeline component', () => {
    describe('renders correctly', () => {
        it('renders without crashing', () => {
            const component = renderer.create(<PersonalTimeline data={data} />);
            expect(component.toJSON()).toMatchSnapshot();
        });

        it('alway render xAxis', () => {
            const component = shallow(<PersonalTimeline data={data} />);
            expect(component.find('.xaxes').length).toBeGreaterThan(0);
        });

        it('renders without timeline node', () => {
            const component = shallow(<PersonalTimeline data={{ records: [] }} />);
            expect(component.find(TimelineNode)).toHaveLength(0);
        });

        it('render TimelineNode correctly', () => {
            const component = shallow(<PersonalTimeline data={data} />);
            expect(component.find(TimelineNode)).toHaveLength(records.length);
        });

        it('render next prev 2 buttons', () => {
            const component = shallow(<PersonalTimeline data={data} />);
            expect(component.find('button')).toHaveLength(2);
        });

        it('shouldComponentUpdate will return right value', () => {
            const wrapper = shallow(<PersonalTimeline data={data} />);
            let nextProps;
            let shouldUpdate;

            nextProps = { data };
            shouldUpdate = wrapper.instance().shouldComponentUpdate(nextProps);
            expect(shouldUpdate).toBe(false);

            nextProps = {
                prevDisabled: true,
                nextDisabled: true,
                records: [],
                pageIndex: 2
            };
            shouldUpdate = wrapper.instance().shouldComponentUpdate(nextProps);
            expect(shouldUpdate).toBe(true);
        });
    });

    describe('behavior tests', () => {
        let component;
        beforeEach(() => {
            component = new PersonalTimeline({ data: data, onPageChange: jest.fn() });
        });

        it('on click previous button', () => {
            component.onClickPrevious();
            expect(component.props.onPageChange).toBeCalledWith(3);
        });

        it('on click next button', () => {
            component.onClickNext();
            expect(component.props.onPageChange).toBeCalledWith(1);
        });
    });
});