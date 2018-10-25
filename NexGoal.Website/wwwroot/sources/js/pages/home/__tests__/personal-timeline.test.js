import React from 'react';
import PersonalTimeline from '../timeline/PersonalTimeline';
import TimelineNode from '../timeline/TimelineNode';
import renderer from 'react-test-renderer';
import { shallow, mount } from 'enzyme';
import axios from 'axios';

jest.mock('axios');

const records = [{
    performanceRecordId: 1,
    gradeId: 2,
    grade: { name: '' },
    comment: '',
    createdDate: '',
    employee: { nickName: 'Oliver' },
    creator: { nickName: 'Michael', avatar: '' }
}];

describe('Test timeline component', () => {
    beforeEach(() => {
        axios.get.mockImplementation(() =>
            Promise.resolve({
                data: {
                    performanceRecords: records,
                    isOverPCI: false
                }
            })
        );
    });

    afterEach(() => {
        axios.get.mockReset();
    });

    describe('renders correctly', () => {
        it('renders without crashing', () => {
            const component = renderer.create(<PersonalTimeline />);
            expect(component.toJSON()).toMatchSnapshot();
        });

        it("should fetch data when mount", async () => {
            const wrapper = mount(<PersonalTimeline />);
            expect(axios.get).toHaveBeenCalledTimes(1);
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
            expect(component.find(TimelineNode)).toHaveLength(0);
        });

        it('render TimelineNode correctly', () => {
            const component = shallow(<PersonalTimeline />);
            component.setState({
                performanceRecords: records
            });
            expect(component.find(TimelineNode)).toHaveLength(records.length);
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
                pageIndex: 0
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

    describe('component will call right method', () => {
        it('call get PCI axios click button', () => {
            let component = shallow(<PersonalTimeline />);
            component.setState({
                performanceTimeline: records
            });
            component.onClickPrevious = jest.fn();
            component.onClickNext = jest.fn();

            const spy = jest.spyOn(component.instance(), 'getPCI');
            component.update();
            component.find('.previous').simulate('click');
            expect(spy).toHaveBeenCalledTimes(1);
        });
    });

    describe('onClick Next Prev Button', () => {
        const setUpComponent = () => {
            const component = shallow(<PersonalTimeline />);
            component.setState({
                pageIndex: 5,
                performanceTimeline: records
            });

            component.onClickPrevious = jest.fn();
            component.onClickNext = jest.fn();
            return component;
        };

        it('prev button when click increase pageIndex state', () => {
            let component = setUpComponent();
            component.find('.previous').simulate('click');
            expect(component.state().pageIndex).toEqual(6);
        });

        it('next button when click call descrease pageIndex state', () => {
            let component = setUpComponent();

            component.find('.next').simulate('click');
            expect(component.state().pageIndex).toEqual(4);
        });
    });
});