import React from 'react';
import TimelineNode from '../timeline/TimelineNode';
import renderer from 'react-test-renderer';

const record = {
    performanceRecordId: 999,
    gradeId: 2,
    grade: { name: '' },
    comment: '',
    createdDate: '',
    employee: { nickName: 'Oliver' },
    creator: { nickName: 'Michael', avatar: '' }
};

const position = 80;

it('render correctly', () => {
    const component = renderer.create(<TimelineNode data={record} position={position} />);
    expect(component.toJSON()).toMatchSnapshot();
});

describe('on click event handler', () => {
    it('onClick Handle should be called with correct params', () => {
        const component = new TimelineNode({ data: record, position: position, onClick: jest.fn() });
        component.handleClick();
        expect(component.props.onClick).toBeCalledWith(999);
    });
});