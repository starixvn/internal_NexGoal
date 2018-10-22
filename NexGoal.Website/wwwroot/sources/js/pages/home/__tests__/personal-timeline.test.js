import React from 'react';
import PersonalTimeline from '../PersonalTimeline';
import TimelineNode from '../TimelineNode';
import renderer from 'react-test-renderer';
import { shallow, mount, configure } from 'enzyme';

const records = [{
    performanceRecordId: 1,
    gradeId: 2,
    grade: { name: '' },
    comment: '',
    createdDate: '',
    employee: { nickName: 'Oliver' },
    creator: { nickName: 'Michael', avatar: '' }
}];

describe('renders correctly', () => {
    it('renders without crashing', () => {
        const component = renderer.create(<PersonalTimeline />);
        expect(component.toJSON()).toMatchSnapshot();
    });

    it('alway render xAxis', () => {
        const component = shallow(<PersonalTimeline />);
        expect(component.find('.xaxes').length).toBeGreaterThan(0);
    });

    it('renders without timeline node', () => {
        const component = shallow(<PersonalTimeline />);
        component.setState({
            performanceRecords: []
        });
        expect(component.find(TimelineNode).length).toBe(0);
    });

    it('render TimelineNode correctly', () => {
        const component = shallow(<PersonalTimeline />);
        component.setState({
            performanceRecords: records
        });
        expect(component.find(TimelineNode).length).toBe(records.length);
    });

    it('render correctly when click 2 button', () => {
        const component = renderer.create(<PersonalTimeline />);
        let tree = component.toJSON();
        tree.props.onClickPrevious;
        tree = component.toJSON();
        expect(tree).toMatchSnapshot();
        tree.props.onClickNext;
        tree = component.toJSON();
        expect(tree).toMatchSnapshot();
    });

    it('render next prev 2 buttons', () => {
        const component = shallow(<PersonalTimeline />);
        expect(component.find('button')).toHaveLength(2);
    });

    it('render disabled Next button when page is 1', () => {
        const component = shallow(<PersonalTimeline />);
        component.setState({
            pageIndex: 1
        });
        expect(component.find('.next').props('disabled').disabled).toBe(true);
    });

    it('render disabled Next Prev button with disabled class', () => {
        const component = shallow(<PersonalTimeline />);
        component.setState({
            prevDisabled: true,
            nexDisabled: true
        });
        expect(component.find('.next').props().disabled).toEqual(true);
        expect(component.find('.previous').props().disabled).toEqual(true);
    });
});

describe('onClick Next Prev Button', () => {
    const component = shallow(<PersonalTimeline />);
    beforeEach(() => {
        component.setState({
            pageIndex: 5,
            performanceTimeline: records
        });
        component.onClickPrevious = jest.fn();
        component.onClickNext = jest.fn();
    });

    it('prev button when click increase pageIndex state', () => {
        component.find('.previous').simulate('click');
        expect(component.state().pageIndex).toEqual(6);
    });

    it('next button when click call descrease pageIndex state', () => {
        component.find('.next').simulate('click');
        expect(component.state().pageIndex).toEqual(4);
    });
});

describe('component will call right method', () => {
    it('call get PCI when click button', () => {
        const component = shallow(<PersonalTimeline />);
        component.setState({
            pageIndex: 5,
            performanceTimeline: records
        });
        component.onClickPrevious = jest.fn();
        component.onClickNext = jest.fn();

        const spy = jest.spyOn(component.instance(), 'getPCI');
        component.update();
        component.find('.previous').simulate('click');
        expect(spy).toHaveBeenCalled();

        component.find('.next').simulate('click');
        expect(spy).toHaveBeenCalled();
    });
});